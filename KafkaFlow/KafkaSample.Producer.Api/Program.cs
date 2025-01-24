using System.Reflection;
using KafkaSample.Framework.Infrastructure;
using KafkaSample.Repositories;
using KafkaSample.WriteStack;
using KafkaSample.WriteStack.Commands;
using KafkaSample.WriteStack.Consumers;
using KafkaFlow;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.ReadStack.Queries;
using MediatR;
using Polly.Utilities;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IConnectionMultiplexer multiplexer = ConnectionMultiplexer
    .Connect("localhost:6379,allowAdmin=true");
builder.Services.AddSingleton(multiplexer);

builder.Services.AddKafkaFlowHostedService(kafkaConfigBuilder =>
{
    kafkaConfigBuilder.AddCluster(clusterConfBuilder =>
    {
        clusterConfBuilder.WithBrokers(new[] { "localhost:29092", "localhost:29093", "localhost:29094" });
        clusterConfBuilder.WithName("KafkaSampleWriteStack");

        clusterConfBuilder.AddProducer("Transactions", producerConfigBuilder => 
            producerConfigBuilder.DefaultTopic("Transactions")
                .AddMiddlewares(m => m
                    .AddSerializer<JsonCoreSerializer>()));
        
        clusterConfBuilder.AddProducer("NewTransactions", producerConfigBuilder => 
            producerConfigBuilder.DefaultTopic("NewTransactions")
                .AddMiddlewares(m => m
                    .AddSerializer<JsonCoreSerializer>()));
        
        clusterConfBuilder.AddConsumer(consumer =>
        {
            consumer.Topic("NewTransactions")
                .WithGroupId("KafkaSample.WriteStack")
                .WithBufferSize(1)
                .WithWorkersCount(1)
                .AddMiddlewares(builder =>
                {
                    builder.RetryForever(retry => retry
                            .Handle(RetryIsRequired)
                            .WithTimeBetweenTriesPlan(static (tryCount) =>
                                new[] {
                                        TimeSpan.FromSeconds(1),
                                        TimeSpan.FromSeconds(2),
                                        TimeSpan.FromSeconds(4)
                                    }
                                    [tryCount % 3]
                            ))
                        .AddSingleTypeDeserializer<Transaction, NewtonsoftJsonDeserializer>(
                            resolver => new NewtonsoftJsonDeserializer())
                        .AddTypedHandlers(typedHandlerConfigurationBuilder =>
                        {
                            typedHandlerConfigurationBuilder.WithHandlerLifetime(InstanceLifetime.Transient)
                                
                                .AddHandler<NewTransactionConsumer>();
                        });
                });

            static bool RetryIsRequired(RetryContext context)
            {
                return 
                    context.Exception is ArgumentNullException ||
                    context.Exception.Message.Contains("Task was cancelled.");
            }
        });
    });
});

builder.Services.AddMediatR(cfg=>
        cfg.RegisterServicesFromAssemblies([Assembly.GetExecutingAssembly(),
            typeof(GetAllTransactionsQuery).Assembly,
            typeof(TransactionCreateCommand).Assembly]));
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationWriteDbContext>();
builder.Services.AddDbContext<ApplicationReadDbContext>();
builder.Services.AddScoped<TransactionRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationWriteDbContext>();
    var dbRead = scope.ServiceProvider.GetRequiredService<ApplicationReadDbContext>();
    db.Database.EnsureCreated();
    dbRead.Database.EnsureCreated();
}

app.MapPost("/transactions/add", async (IMediator mediator, TransactionCreateCommand command, CancellationToken cancellationToken) => 
    await mediator.Send(command, cancellationToken));

app.MapGet("/", () => "Hello World!");

app.Run();