using System.Net.Http.Json;
using System.Text.Json;

var baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "http://localhost:5242";
var apiKey = args.Length > 3 ? args[3] : Environment.GetEnvironmentVariable("BIKEIOT_API_KEY");
var parsedBikeId = 1;
var parsedIsLock = false;
var shouldInvokeLockAction = args.Length > 2
    && int.TryParse(args[1], out parsedBikeId)
    && bool.TryParse(args[2], out parsedIsLock);
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
