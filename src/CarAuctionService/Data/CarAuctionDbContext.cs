using CarAuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Data
{
    public class CarAuctionDbContext : DbContext
    {
        public CarAuctionDbContext(DbContextOptions options) : base(options)
        {           
        }
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
