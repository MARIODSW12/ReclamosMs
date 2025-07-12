using MediatR;

using Reclamos.Application.DTOs;

namespace Reclamos.Application.Commands
{

    public class SolveClaimCommand : IRequest<String>
    {
        public SolveClaimDto ClaimDto { get; }

        public SolveClaimCommand(SolveClaimDto claimDto)
        {
            ClaimDto = claimDto ?? throw new ArgumentNullException(nameof(claimDto));
        }
    }
}
