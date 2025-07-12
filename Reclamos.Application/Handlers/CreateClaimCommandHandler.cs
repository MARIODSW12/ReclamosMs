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
    public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, string>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateClaimCommandHandler(IClaimRepository claimRepository, IPublishEndpoint publishEndpoint)
        {
            _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(_publishEndpoint));
        }

        public async Task<string> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var claimId = Guid.NewGuid().ToString();
                Console.WriteLine($"Creating claim with ID: {claimId}");
                var claim = ClaimFactory.Create(
                    new VOId(claimId),
                    new VOUserId(request.ClaimDto.UserId),
                    new VOAuctionId(request.ClaimDto.AuctionId),
                    new VOMotive(request.ClaimDto.Motive),
                    new VODescription(request.ClaimDto.Description),
                    request.Evidence.Select(e => new VOEvidence(e)).ToList(),
                    new VOStatus("pending"),
                    null,
                    null,
                    request.PrizeClaimId == null ? null : new VOPrizeClaimId(request.PrizeClaimId)
                    );
                
                await _claimRepository.CreateClaim(claim);


                var claimCreatedEvent = new ClaimCreatedEvent(
                    claim.Id.Value,
                    claim.UserId.Value,
                    claim.AuctionId.Value,
                    claim.Motive.Value,
                    claim.Description.Value,
                    claim.Evidence.Select(e => e.Value).ToList(),
                    claim.Status.Value,
                    claim.PrizeClaimId == null ? null : claim.PrizeClaimId.Value
                );
                Console.WriteLine($"Publishing ClaimCreatedEvent for claim ID: {claim.Id.Value}");
                await _publishEndpoint.Publish(claimCreatedEvent);

                return claim.Id.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}