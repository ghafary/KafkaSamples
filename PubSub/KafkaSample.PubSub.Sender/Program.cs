// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;

Console.WriteLine("Producer Config");

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",// Kafka broker address
    SecurityProtocol = SecurityProtocol.SaslPlaintext, // Use SASL_PLAINTEXT
    SaslMechanism = SaslMechanism.Plain, // Use PLAIN mechanism
    SaslUsername = "admin",             // Replace with your username
    SaslPassword = "123"
};

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    try
    {
        for (int i = 0; i < 10; i++)
        {
            var message = new Message<Null, string> { Value = $"Message {i}" };

            var deliveryResult = await producer.ProduceAsync("test-topic", message);

            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
    }
    catch (ProduceException<Null, string> ex)
    {
        Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
    }
}

Console.ReadLine();
