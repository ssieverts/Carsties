using AuctionService.Data;
using Azure;
using Grpc.Core;

namespace AuctionService;

public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _dbContext;

    public GrpcAuctionService(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GrpcAuctionResponse> GetAuction(
        GetAuctionRequest request,
        ServerCallContext context
    )
    {
        Console.WriteLine("==>> Received Grpc request for auction.");

        var auction =
            await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));

        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = request.Id,
                ReservPrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };

        Console.WriteLine("==>> Returning auction to Grpc client.");

        return response;
    }
}
