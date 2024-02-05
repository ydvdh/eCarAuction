using MongoDB.Driver;
using MongoDB.Entities;

namespace BiddingAuctionService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("BidDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("BidDbConnection")));
        }
    }
}
