using FluxPay.Payment.Domain;
using FluxPay.Payment.Infrastructure;
using FluxPay.Payment.Middleware;
using FluxPay.Payment.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// Configuration
// ------------------------------------

var redisConnection =
    builder.Configuration["Redis:Connection"]
    ?? builder.Configuration["REDIS_CONNECTION"]
    ?? "localhost:6379";

// ------------------------------------
// Services
// ------------------------------------

builder.Services.AddControllers();

var sqlConnection =
    builder.Configuration.GetConnectionString("SqlConnection");

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(sqlConnection, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));

builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IFraudService, FraudService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect($"{redisConnection},abortConnect=false"));

builder.Services.AddHealthChecks()
    .AddSqlServer(sqlConnection!, name: "sqlServer")
    .AddRedis(redisConnection, name: "redis");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------
// App Pipeline
// ------------------------------------

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// ------------------------------------
// Auto Migrations + Seed
// ------------------------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

    db.Database.Migrate();

    if (!db.ApiKeys.Any())
    {
        db.ApiKeys.Add(new ApiKey
        {
            Id = Guid.NewGuid(),
            Key = "sk_test_abc123",
            TenantId = "tenant_123",
            IsActive = true
        });

        db.SaveChanges();
    }
}

app.Run();