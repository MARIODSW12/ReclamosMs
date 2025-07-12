using MassTransit;
using MongoDB.Bson;

using Reclamos.Domain.Events;

using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Consumer
{
    public class SolveClaimConsumer(IServiceProvider serviceProvider, IClaimReadRepository claimReadRepository) : IConsumer<ClaimSolvedEvent>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IClaimReadRepository _claimReadRepository = claimReadRepository;

        public async Task Consume(ConsumeContext<ClaimSolvedEvent> @event)
        {
            try
            {
                var message = @event.Message;
                var claim = await _claimReadRepository.GetReclamoPorId(message.ClaimId);

                claim["status"] = "solved";
                claim["solution"] = message.Solution;
                claim["solutionDetail"] = message.SolutionDetail;

                await _claimReadRepository.ResolverReclamo(claim);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}