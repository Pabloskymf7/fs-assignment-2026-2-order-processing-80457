using MassTransit;
using Serilog;
using ShippingService.Consumers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/shipping-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithProperty("ServiceName", "ShippingService")
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentApprovedConsumer>();

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

Log.Information("ShippingService starting...");

await host.RunAsync();