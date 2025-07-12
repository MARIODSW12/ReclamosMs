using MassTransit;
using MongoDB.Bson;

using Reclamos.Domain.Events;

using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Consumer
{
    public class CreateClaimConsumer(IServiceProvider serviceProvider, IClaimReadRepository claimReadRepository) : IConsumer<ClaimCreatedEvent>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IClaimReadRepository _claimReadRepository = claimReadRepository;

        public async Task Consume(ConsumeContext<ClaimCreatedEvent> @event)
        {
            try
            {
                var message = @event.Message;
                var bsonClaim = new BsonDocument
                {
                    { "_id", message.Id},
                    { "userId", message.UserId},
                    { "auctionId", message.AuctionId},
                    { "prizeClaimId", message.PrizeClaimId == null ? BsonNull.Value : message.PrizeClaimId },
                    { "motive", message.Motive},
                    { "description", message.Description},
                    { "evidence", message.Evidence.Count > 0 ? new BsonArray(message.Evidence) : new BsonArray() },
                    { "status", message.Status},
                    { "solution", BsonNull.Value},
                    { "solutionDetail", BsonNull.Value}
                };

                await _claimReadRepository.AgregarReclamo(bsonClaim);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}