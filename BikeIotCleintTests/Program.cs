using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Internal;

namespace BikeIotClientTests
{
    internal class Program
    {
        static async Task  Main(string[] args)
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithClientId("test-device-001")
                .Build();

            await client.ConnectAsync(options);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("devices/test-device/telemetry")
                .WithPayload("{\"temperature\":24.9}")
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await client.PublishAsync(message);

            Console.WriteLine("Message sent");
            await client.DisconnectAsync();
        }
    }
}
