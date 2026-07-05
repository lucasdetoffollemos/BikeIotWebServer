using MQTTnet;
using MQTTnet.Client;

namespace BikeIotWebServer.mqtt
{
    public class MqttSubscriberService : IHostedService
    {
        private const string TopicFilter = "devices/+/telemetry";
        private IMqttClient? _mqttClient;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();

                Console.WriteLine($"MQTT message received. Topic: {topic}, Payload: {payload}");
                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithClientId($"bike-web-subscriber-{Guid.NewGuid():N}")
                .Build();

            await _mqttClient.ConnectAsync(options, cancellationToken);

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(TopicFilter)
                .Build(), cancellationToken);

            Console.WriteLine($"MQTT subscriber connected on localhost:1883 ({TopicFilter}).");
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
