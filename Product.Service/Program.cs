using ECommerce.Shared.Authentication;
using ECommerce.Shared.Infrastructure.Outbox;
using ECommerce.Shared.Infrastructure.RabbitMq;
using ECommerce.Shared.Observability;
using Product.Service.Endpoints;
using Product.Service.Infrastructure.Data.EntityFramework;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSqlServerDatastore(builder.Configuration);
builder.Services.AddRabbitMqEventBus(builder.Configuration)
    .AddRabbitMqEventPublisher();
builder.Services.AddOpenTelemetryTracing(builder.Configuration,"Product", 
    providerBuilder => providerBuilder.WithSqlInstrumentation());
builder.Services.AddOutbox(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
    app.ApplyOutboxMigrations();
}

app.RegisterEndpoints();
app.UseHttpsRedirection();
app.UseJwtAuthentication();

app.Run();

public partial class Program{
}
