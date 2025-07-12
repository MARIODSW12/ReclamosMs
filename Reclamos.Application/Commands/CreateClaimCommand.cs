using MediatR;

using Reclamos.Application.DTOs;

namespace Reclamos.Application.Commands
{

    public class CreateClaimCommand : IRequest<String>
    {
        public CreateClaimDto ClaimDto { get; }
        public List<string> Evidence { get; }
        public string? PrizeClaimId { get; }

        public CreateClaimCommand(CreateClaimDto claimDto, List<string> evidence, string? prizeClaimId)
        {
            ClaimDto = claimDto ?? throw new ArgumentNullException(nameof(claimDto));
            Evidence = evidence;
            PrizeClaimId = prizeClaimId;
        }
    }
}