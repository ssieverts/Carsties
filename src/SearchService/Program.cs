using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.UseMessageRetry(r =>
            {
                r.Handle<RabbitMqConnectionException>();
                r.Interval(5, TimeSpan.FromSeconds(10));
            });

            cfg.Host(
                builder.Configuration["RabbitMq:Host"],
                "/",
                host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                }
            );
            cfg.ReceiveEndpoint(
                "search-auction-created",
                e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));
                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                    e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
                    e.ConfigureConsumer<AuctionDeletedConsumer>(context);
                }
            );

            cfg.ConfigureEndpoints(context);
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.MapControllers();

Policy
    .Handle<TimeoutException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10))
    .ExecuteAndCapture(async () => await DbInitializer.InitDb(app));

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}
