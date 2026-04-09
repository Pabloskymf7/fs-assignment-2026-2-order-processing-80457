using InventoryService.Consumers;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/orderapi-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithProperty("ServiceName", "OrderManagement.API")
    .Enrich.WithCorrelationId()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var host = builder.Build();

Log.Information("InventoryService starting...");

await host.RunAsync();