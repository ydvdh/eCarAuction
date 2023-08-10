using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchAuctionService.Models;
using SearchAuctionService.RequestHelpers;

namespace SearchAuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            if(!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(i => i.Ascending(m => m.Make)),
                "new" => query.Sort(i => i.Descending(c => c.CreatedAt)),
                _ => query.Sort(i => i.Ascending(a => a.AuctionEnd))
            };
            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }
            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Seller == searchParams.Winner);
            }

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

            var result = await query.ExecuteAsync();

            return Ok(new { result = result.Results, pageCount = result.PageCount, totalCount = result.TotalCount});
        }
    }
}
