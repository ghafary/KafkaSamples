
using Confluent.Kafka;

Console.WriteLine("Consumer Config!");

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092", // Kafka broker address
    GroupId = "test-consumer-group",     // Consumer group ID
    AutoOffsetReset = AutoOffsetReset.Earliest, // Start reading from the beginning if no offset is found
    EnableAutoCommit = true,
    SecurityProtocol = SecurityProtocol.SaslPlaintext, // Use SASL_PLAINTEXT
    SaslMechanism = SaslMechanism.Plain, // Use PLAIN mechanism
    SaslUsername = "admin",             // Replace with your username
    SaslPassword = "123"// Automatically commit offsets
};

using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
{
    consumer.Subscribe("test-topic");

    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => {
        e.Cancel = true; // Prevent default termination
        cts.Cancel();
    };

    try
    {
        while (true)
        {
            var consumeResult = consumer.Consume(cts.Token);
            Console.WriteLine($"Consumed message '{consumeResult.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
        }
    }
    catch (OperationCanceledException)
    {
        // Exit gracefully on Ctrl+C
    }
    finally
    {
        consumer.Close(); // Ensure offsets are committed before exiting
    }
}

