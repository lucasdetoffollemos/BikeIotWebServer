
using MQTTnet;
using MQTTnet.Server;


namespace BikeIotWebServer.mqtt
{
    public class MqttService : IHostedService
    {
        private MqttServer? _mqttServer = null;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();

            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .Build();

            _mqttServer = factory.CreateMqttServer(options);

            var message = new MqttApplicationMessageBuilder().WithTopic("HelloWorld").WithPayload("Test").Build();



            _mqttServer.ApplicationMessageNotConsumedAsync += e =>
            {
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                var topic = e.ApplicationMessage.Topic;

                Console.WriteLine($"📡 {topic}: {payload}");

                // TODO:
                // Save to DB
                // Push to SignalR
                // Trigger alerts

                return Task.CompletedTask;
            };

            await _mqttServer.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _mqttServer.StopAsync();
        }
    }
}
