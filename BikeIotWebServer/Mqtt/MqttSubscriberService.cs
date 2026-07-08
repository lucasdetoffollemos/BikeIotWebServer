using System.Text.Json;
using BikeIotWebServer.Services;
using BikeIotWebServer.ViewModels;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

namespace BikeIotWebServer.mqtt
{
    public class MqttSubscriberService : IHostedService
    {
        private const string TopicFilter = "devices/+/telemetry";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private IMqttClient? _mqttClient;

        public MqttSubscriberService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();

                if (string.IsNullOrWhiteSpace(payload))
                {
                    Console.WriteLine($"MQTT message received with empty payload. Topic: {topic}");
                    return;
                }

                try
                {
                    var telemetry = JsonSerializer.Deserialize<BikeTelemetry>(payload);

                    if (telemetry is null)
                    {
                        Console.WriteLine($"MQTT message could not be deserialized. Topic: {topic}, Payload: {payload}");
                        return;
                    }

                    using var scope = _serviceScopeFactory.CreateScope();
                    var bikeTelemetryService = scope.ServiceProvider.GetRequiredService<BikeTelemetryService>();

                    await bikeTelemetryService.SaveAsync(telemetry);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"MQTT payload is invalid JSON. Topic: {topic}, Error: {ex.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save MQTT telemetry. Topic: {topic}, Error: {ex.Message}");
                    return;
                }

                Console.WriteLine($"MQTT message received. Topic: {topic}, Payload: {payload}");
            };

            var mqttHost = _configuration["Mqtt:Host"] ?? "localhost";
            var mqttPort = _configuration.GetValue<int?>("Mqtt:Port") ?? 1883;
            var mqttUsername = _configuration["Mqtt:Username"];
            var mqttPassword = _configuration["Mqtt:Password"];

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttHost, mqttPort)
                .WithClientId($"bike-web-subscriber-{Guid.NewGuid():N}");

            if (!string.IsNullOrWhiteSpace(mqttUsername))
            {
                optionsBuilder = optionsBuilder.WithCredentials(mqttUsername, mqttPassword);
            }

            var options = optionsBuilder.Build();

            await _mqttClient.ConnectAsync(options, cancellationToken);

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(TopicFilter)
                .Build(), cancellationToken);

            Console.WriteLine($"MQTT subscriber connected on {mqttHost}:{mqttPort} ({TopicFilter}).");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient is not null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
