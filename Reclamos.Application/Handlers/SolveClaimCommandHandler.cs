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
    public class SolveClaimCommandHandler : IRequestHandler<SolveClaimCommand, string>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public SolveClaimCommandHandler(IClaimRepository claimRepository, IPublishEndpoint publishEndpoint)
        {
            _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(_publishEndpoint));
        }

        public async Task<string> Handle(SolveClaimCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var dto = request.ClaimDto;
                var claim = await _claimRepository.GetClaimById(dto.ClaimId);
                if (claim == null)
                {
                    throw new ArgumentException($"Claim with ID {dto.ClaimId} not found.");
                }
                claim.SolveClaim(dto.Solution, dto.SolutionDetail);

                await _claimRepository.SolveClaim(claim);

                var claimUpdatedEvent = new ClaimSolvedEvent(
                    claim.Id.Value, claim.Solution.Value, claim.SolutionDetail.Value
                );
                await _publishEndpoint.Publish(claimUpdatedEvent);

                return claim.Id.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}