using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchAuctionService.Models;

namespace SearchAuctionService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("----> Consuming auction created:", context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);
            if (item.Model == "Food") throw new ArgumentException("Cannot sell item of model FOO");
            await item.SaveAsync();
        }
    }
}
