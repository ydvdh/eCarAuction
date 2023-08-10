using MongoDB.Entities;
using SearchAuctionService.Models;

namespace SearchAuctionService.Services
{
    public class AuctionHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuctionHttpClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Item>> GetItemsForSearchDbAsync()
        {
            var lastUpdated = await DB.Find<Item, string>().Sort(x=>x.Descending(x=>x.UpdatedAt))
                                      .Project(x => x.UpdatedAt.ToString()).ExecuteFirstAsync();

            return await _httpClient.GetFromJsonAsync<List<Item>>(_configuration["CarAuctionService"] + "api/auctions?date=" + lastUpdated);
        }
    }
}
