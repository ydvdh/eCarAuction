using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarAuctionService.Data;
using CarAuctionService.DTOs;
using CarAuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly CarAuctionDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public AuctionsController(CarAuctionDbContext dbContext, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        {
            var query = _dbContext.Auctions.OrderBy(i=>i.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x=>x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0 );
            }
            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            var auction = await _dbContext.Auctions.Include(i => i.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null) return NotFound();
            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);

            auction.Seller = "test";
            _dbContext.Add(auction);
            var result = await _dbContext.SaveChangesAsync() > 0;

            var newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            if (!result) return BadRequest("Could not save changes to DB");

            return CreatedAtAction(nameof(GetAllAuctions), new { auction.Id }, newAuction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _dbContext.Auctions.Include(i => i.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null) return NotFound();

            // Check if user is seller

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _dbContext.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest("Problem saving the changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _dbContext.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            // TODO: Check seller == user

            _dbContext.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await _dbContext.SaveChangesAsync() > 0;
            if (!result) return BadRequest("Couldn't update the changes to DB");
            return Ok();
        }
    }
}
