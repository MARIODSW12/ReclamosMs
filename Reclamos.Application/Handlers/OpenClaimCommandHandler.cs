using MediatR;

using Reclamos.Application.Commands;

using Reclamos.Domain.Repositories;
using Reclamos.Domain.ValueObjects;
using Reclamos.Domain.Aggregates;
using Reclamos.Domain.Events;
using Reclamos.Domain.Factories;
using MassTransit;
using MassTransit.Transports;

namespace Reclamos.Application.Handlers
{
    public class OpenClaimCommandHandler : IRequestHandler<OpenClaimCommand, string>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public OpenClaimCommandHandler(IClaimRepository claimRepository, IPublishEndpoint publishEndpoint)
        {
            _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(_publishEndpoint));
        }

        public async Task<string> Handle(OpenClaimCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var claim = await _claimRepository.GetClaimById(request.ClaimId);
                if (claim == null)
                {
                    throw new ArgumentException($"Claim with ID {request.ClaimId} not found.");
                }
                claim.Openclaim();

                await _claimRepository.OpenClaim(claim);

                var claimOpenedEvent = new ClaimOpenedEvent(
                    claim.Id.Value
                );
                await _publishEndpoint.Publish(claimOpenedEvent);

                return claim.Id.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}