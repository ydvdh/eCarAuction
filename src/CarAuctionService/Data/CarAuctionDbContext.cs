using CarAuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Data
{
    public class CarAuctionDbContext : DbContext
    {
        public CarAuctionDbContext(DbContextOptions options) : base(options)
        {           
        }
        public DbSet<Auction> Auctions { get; set; }
    }
}
