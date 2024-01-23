﻿using AuctionService;
using Grpc.Net.Client;

namespace BiddingService;

public class GrpcAuctionClient
{
    private readonly ILogger<GrpcAuctionClient> _logger;
    private readonly IConfiguration _config;

    public GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public Auction GetAuction(string auctionId){

        _logger.LogInformation("Calling Grpc Service for auction {auctionId}", auctionId);

        var channel = GrpcChannel.ForAddress(_config["GrpcAuction"]);
        var client = new GrpcAuction.GrpcAuctionClient(channel);
        var request = new GetAuctionRequest { Id = auctionId };

        try
        {
            var reply = client.GetAuction(request);
            var auction = new Auction{
                ID = reply.Auction.Id,
                AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                ReservePrice = reply.Auction.ReservPrice,
                Seller = reply.Auction.Seller
            };

            return auction;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error calling Grpc Service for auction {auctionId}", auctionId);
            return null;
        }
    }
}
