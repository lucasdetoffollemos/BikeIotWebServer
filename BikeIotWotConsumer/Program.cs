using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

var baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "http://localhost:5242";
var isPollingMode = args.Length > 1 && string.Equals(args[1], "poll", StringComparison.OrdinalIgnoreCase);
var apiKey = ResolveApiKey(args, isPollingMode);
var parsedBikeId = 1;
var parsedIsLock = false;
var pollingBikeId = 1;
var pollingIntervalSeconds = 5;
var shouldInvokeLockAction = !isPollingMode
    && args.Length > 2
    && int.TryParse(args[1], out parsedBikeId)
    && bool.TryParse(args[2], out parsedIsLock);
var shouldPollTelemetry = isPollingMode
    && args.Length > 3
    && int.TryParse(args[2], out pollingBikeId)
    && int.TryParse(args[3], out pollingIntervalSeconds)
    && pollingIntervalSeconds > 0;
var bikeId = shouldInvokeLockAction ? parsedBikeId : 1;
var isLock = shouldInvokeLockAction && parsedIsLock;

using var httpClient = new HttpClient();

if (!string.IsNullOrWhiteSpace(apiKey))
{
    httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
}

var discovery = await httpClient.GetFromJsonAsync<DiscoveryDocument>($"{baseUrl}/.well-known/wot");
if (discovery?.Things is null || discovery.Things.Count == 0)
{
    Console.WriteLine("No Things discovered.");
    return;
}

var thing = discovery.Things[0];
Console.WriteLine($"Thing discovered: {thing.Title} ({thing.Href})");

var tdJson = await httpClient.GetStringAsync(thing.Href);
using var tdDocument = JsonDocument.Parse(tdJson);
var root = tdDocument.RootElement;

var title = root.GetProperty("title").GetString() ?? "Unknown Thing";
Console.WriteLine($"Thing title: {title}");

if (isPollingMode)
{
    if (!shouldPollTelemetry)
    {
        Console.WriteLine("Polling mode usage: <baseUrl> poll <bikeId> <intervalSeconds> [apiKey]");
        return;
    }

    await PollTelemetryHistoryAsync(root, httpClient, pollingBikeId, pollingIntervalSeconds);
    return;
}

var healthUrl = root
    .GetProperty("properties")
    .GetProperty("health")
    .GetProperty("forms")[0]
    .GetProperty("href")
    .GetString();

if (!string.IsNullOrWhiteSpace(healthUrl))
{
    var health = await httpClient.GetFromJsonAsync<bool>(healthUrl);
    Console.WriteLine($"Health: {health}");
}

var setLockUrl = root
    .GetProperty("actions")
    .GetProperty("setLockState")
    .GetProperty("forms")[0]
    .GetProperty("href")
    .GetString();

if (shouldInvokeLockAction && !string.IsNullOrWhiteSpace(setLockUrl))
{
    var response = await httpClient.PostAsJsonAsync(setLockUrl, new { bikeId, isLock });
    response.EnsureSuccessStatusCode();
    Console.WriteLine($"setLockState invoked for bikeId={bikeId}, isLock={isLock}");
}
else
{
    Console.WriteLine("Lock action not invoked. Pass <baseUrl> <bikeId> <true|false> [apiKey] to invoke setLockState.");
}

var getLockUrlTemplate = root
    .GetProperty("actions")
    .GetProperty("getLockState")
    .GetProperty("forms")[0]
    .GetProperty("href")
    .GetString();

if (shouldInvokeLockAction && !string.IsNullOrWhiteSpace(getLockUrlTemplate))
{
    var getLockUrl = getLockUrlTemplate.Replace("{bikeId}", bikeId.ToString(), StringComparison.Ordinal);
    var lockStateJson = await httpClient.GetStringAsync(getLockUrl);
    Console.WriteLine($"Lock state: {lockStateJson}");
}

static string? ResolveApiKey(string[] args, bool isPollingMode)
{
    if (isPollingMode)
    {
        return args.Length > 4 ? args[4] : Environment.GetEnvironmentVariable("BIKEIOT_API_KEY");
    }

    return args.Length > 3 ? args[3] : Environment.GetEnvironmentVariable("BIKEIOT_API_KEY");
}

static async Task PollTelemetryHistoryAsync(JsonElement root, HttpClient httpClient, int bikeId, int intervalSeconds)
{
    var telemetryHistoryUrl = root
        .GetProperty("properties")
        .GetProperty("telemetryHistory")
        .GetProperty("forms")[0]
        .GetProperty("href")
        .GetString();

    if (string.IsNullOrWhiteSpace(telemetryHistoryUrl))
    {
        Console.WriteLine("telemetryHistory property form was not found.");
        return;
    }

    Console.WriteLine($"Polling telemetryHistory for bikeId={bikeId} every {intervalSeconds}s. Press Ctrl+C to stop.");

    string? lastSeenSignature = null;
    var pollIteration = 0;

    while (true)
    {
        pollIteration++;
        var pollStartedAt = DateTimeOffset.UtcNow;
        var uriBuilder = new UriBuilder(telemetryHistoryUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["bikeId"] = bikeId.ToString();
        query["limit"] = "1";
        query["order"] = "desc";
        uriBuilder.Query = query.ToString() ?? string.Empty;

        try
        {
            var telemetry = await httpClient.GetFromJsonAsync<List<BikeTelemetrySnapshot>>(uriBuilder.Uri);
            var latest = telemetry?.FirstOrDefault();

            if (latest == null)
            {
                Console.WriteLine($"Poll {pollIteration} | {pollStartedAt:O} | no telemetry yet");
            }
            else
            {
                var signature = JsonSerializer.Serialize(latest);
                var status = string.Equals(signature, lastSeenSignature, StringComparison.Ordinal)
                    ? "no change"
                    : "changed";

                Console.WriteLine(
                    $"Poll {pollIteration} | {pollStartedAt:O} | {status} | bikeId={latest.BikeId} | velocidade={latest.Velocidade} | lat={latest.Latitude} | lon={latest.Longitude} | ts={latest.Timestamp:O}");

                lastSeenSignature = signature;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Poll {pollIteration} | {pollStartedAt:O} | error | {ex.Message}");
        }

        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
    }
}

internal sealed class BikeTelemetrySnapshot
{
    public int BikeId { get; set; }
    public float Velocidade { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}

internal sealed class DiscoveryDocument
{
    public List<ThingDiscoveryEntry> Things { get; set; } = [];
}

internal sealed class ThingDiscoveryEntry
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}
