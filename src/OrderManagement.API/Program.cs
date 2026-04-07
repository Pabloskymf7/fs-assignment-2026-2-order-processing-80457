using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Application.Consumers;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/orderapi-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithProperty("ServiceName", "OrderManagement.API")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryResultConsumer>();
    x.AddConsumer<PaymentResultConsumer>();
    x.AddConsumer<ShippingResultConsumer>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "Kayak", Description = "A boat for one person", Price = 275, Stock = 10 },
            new Product { Name = "Lifejacket", Description = "Protective and fashionable", Price = 48.95m, Stock = 50 },
            new Product { Name = "Soccer Ball", Description = "FIFA-approved size and weight", Price = 19.50m, Stock = 100 },
            new Product { Name = "Corner Flags", Description = "Give your playing field a professional touch", Price = 34.95m, Stock = 25 },
            new Product { Name = "Thinking Cap", Description = "Improve brain efficiency by 75%", Price = 16, Stock = 75 },
            new Product { Name = "Unsteady Chair", Description = "Secretly give your opponent a disadvantage", Price = 29.95m, Stock = 30 },
            new Product { Name = "Human Chess Board", Description = "A fun game for the family", Price = 75, Stock = 15 },
            new Product { Name = "Bling-Bling King", Description = "Gold-plated, diamond-studded King", Price = 1200, Stock = 5 }
        );
        db.SaveChanges();
    }
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
