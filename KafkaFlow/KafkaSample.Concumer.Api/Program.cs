using System.Text.Json;
using KafkaSample.Transactions;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
builder.Services.AddSingleton(multiplexer);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaFlowHostedService(kafkaConfigBuilder =>
{
    kafkaConfigBuilder.AddCluster(clusterConfBuilder =>
    {
        clusterConfBuilder.WithBrokers(new[] { "localhost:29092", "localhost:29093", "localhost:29094" });
        clusterConfBuilder.WithName("KafkaSampleWriteStack");

        clusterConfBuilder.AddConsumer(consumer =>
        {
            consumer.Topic("Transactions")
                .WithGroupId("KafkaSample.Transactions1")
                .WithBufferSize(1)
                .WithWorkersCount(1)
                .AddMiddlewares(builder =>
                {
                    builder
                        .AddSingleTypeDeserializer<Transaction, NewtonsoftJsonDeserializer>(
                            resolver => new NewtonsoftJsonDeserializer())
                        .AddTypedHandlers(typedHandlerConfigurationBuilder =>
                        {
                            typedHandlerConfigurationBuilder.WithHandlerLifetime(InstanceLifetime.Transient)
                                .AddHandler<TransactionConsumer>();
                        });
                });

        });
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.MapGet("/products/{id}", async (IConnectionMultiplexer multiplexer, int id, CancellationToken cancellationToken) =>
{
    var data = await multiplexer.GetDatabase().StringGetAsync("products:"+id);
    return JsonSerializer.Deserialize<Transaction>(data);
});

app.MapGet("/products", async (IConnectionMultiplexer multiplexer, CancellationToken cancellationToken) =>
{
    var server = multiplexer.GetServer("localhost", 6379);
    var products = new List<Transaction>();

    await foreach (var key in server.KeysAsync(pattern: "transactions:*"))
    {
        var stringTransaction = await multiplexer.GetDatabase().StringGetAsync(key);
        products.Add(JsonSerializer.Deserialize<Transaction>(stringTransaction));
    }

    return products; 
});



app.Run();