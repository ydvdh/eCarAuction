using CarAuctionService.Data;
using CarAuctionService.Entities;
using Contracts;
using MassTransit;

namespace CarAuctionService.Consumer
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly CarAuctionDbContext _dbContext;

        public AuctionFinishedConsumer(CarAuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("---> Consuming auction finished");

            var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            auction.Status = auction.SoldAmount > auction.ReservePrice? Status.Finished : Status.ReserveNotMet;
            await _dbContext.SaveChangesAsync();
        }
    }
}
