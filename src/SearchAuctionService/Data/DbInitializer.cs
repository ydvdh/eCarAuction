using MongoDB.Driver;
using MongoDB.Entities;
using SearchAuctionService.Models;
using SearchAuctionService.Services;

namespace SearchAuctionService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                       .Key(x => x.Make, KeyType.Text)
                       .Key(x => x.Model, KeyType.Text)
                       .Key(x => x.Color, KeyType.Text)
                       .CreateAsync();

            var count = await DB.CountAsync<Item>();

            using var scope = app.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionHttpClientService>();

            var items = await httpClient.GetItemsForSearchDbAsync();

            Console.WriteLine(items.Count + " Items returned from the auction services");

            if(items.Count > 0) await DB.SaveAsync(items);
        }
    }
}
