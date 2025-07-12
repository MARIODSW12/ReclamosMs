using MediatR;

using Reclamos.Application.DTOs;

namespace Reclamos.Application.Commands
{

    public class OpenClaimCommand : IRequest<String>
    {
        public string ClaimId { get; }

        public OpenClaimCommand(string claimId)
        {
            ClaimId = claimId ?? throw new ArgumentNullException(nameof(claimId));
        }
    }
}
