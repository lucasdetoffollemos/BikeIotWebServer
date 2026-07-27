using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace BikeIotWebServer.mqtt
{
    public class MqttPublisherService
    {
        private readonly IConfiguration _configuration;

        public MqttPublisherService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishBikeLockAsync(int bikeId, bool isLock, CancellationToken cancellationToken = default)
        {
            var factory = new MqttFactory();
            using var mqttClient = factory.CreateMqttClient();

            var mqttHost = _configuration["Mqtt:Host"] ?? "localhost";
            var mqttPort = _configuration.GetValue<int?>("Mqtt:Port") ?? 1883;
            var mqttUsername = _configuration["Mqtt:Username"];
            var mqttPassword = _configuration["Mqtt:Password"];

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttHost, mqttPort)
                .WithClientId($"bike-web-publisher-{Guid.NewGuid():N}");

            if (!string.IsNullOrWhiteSpace(mqttUsername))
            {
                optionsBuilder = optionsBuilder.WithCredentials(mqttUsername, mqttPassword);
            }

            var payload = JsonSerializer.Serialize(new
            {
                bikeId,
                isLock
            });

            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"bikes/{bikeId}/lock")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.ConnectAsync(optionsBuilder.Build(), cancellationToken);

            try
            {
                await mqttClient.PublishAsync(message, cancellationToken);
            }
            finally
            {
                if (mqttClient.IsConnected)
                {
                    await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
                }
            }
        }
    }
}
