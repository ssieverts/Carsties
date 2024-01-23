using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;

public class CheckAuctionFinished : BackgroundService
{
    private ILogger<CheckAuctionFinished> _logger;
    private IServiceProvider _serviceProvider;

    public CheckAuctionFinished(
        ILogger<CheckAuctionFinished> logger,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CheckAuctionFinished is starting.");

        stoppingToken.Register(
            () => _logger.LogInformation("CheckAuctionFinished background task is stopping.")
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        var finishedAuctions = await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished)
            .ExecuteAsync(stoppingToken);

        if (finishedAuctions.Count() == 0)
            return;

        _logger.LogInformation(
            "Found {count} auctions that are finished",
            finishedAuctions.Count()
        );

        using var scope = _serviceProvider.CreateScope();

        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finishedAuctions)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winningBid = await DB.Find<Bid>()
                .Match(x => x.AuctionId == auction.ID)
                .Match(x => x.BidStatus == BidStatus.Accepted)
                .Sort(x => x.Descending(y => y.Amount))
                .ExecuteFirstAsync(stoppingToken);

            await endpoint.Publish(
                new AuctionFinished
                {
                    ItemsSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount,
                    Seller = auction.Seller
                },
                stoppingToken
            );
        }
    }
}
