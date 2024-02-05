using AutoMapper;
using BiddingAuctionService.DTOs;
using BiddingAuctionService.Models;
using Contracts;

namespace BiddingAuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bid, BidDto>();
            CreateMap<Bid, BidPlaced>();
        }
    }
}
