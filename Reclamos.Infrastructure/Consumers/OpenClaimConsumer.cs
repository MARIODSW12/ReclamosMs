using MassTransit;
using MongoDB.Bson;

using Reclamos.Domain.Events;

using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Consumer
{
    public class OpenClaimConsumer(IServiceProvider serviceProvider, IClaimReadRepository claimReadRepository) : IConsumer<ClaimOpenedEvent>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IClaimReadRepository _claimReadRepository = claimReadRepository;

        public async Task Consume(ConsumeContext<ClaimOpenedEvent> @event)
        {
            try
            {
                var message = @event.Message;

                await _claimReadRepository.AbrirReclamo(message.ClaimId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}