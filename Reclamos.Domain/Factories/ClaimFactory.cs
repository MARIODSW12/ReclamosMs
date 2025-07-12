using Reclamos.Domain.Aggregates;
using Reclamos.Domain.ValueObjects;

namespace Reclamos.Domain.Factories
{
    public static class ClaimFactory
    {
        public static Claim Create(VOId id, VOUserId userId, VOAuctionId auctionId, VOMotive motive,
            VODescription description, List<VOEvidence> evidence, VOStatus status, VOSolution? solution, VOSolutionDetail? solutionDetail, VOPrizeClaimId? prizeClaimId)
        {
            return new Claim(id, userId, auctionId, motive, description, evidence, status, solution, solutionDetail, prizeClaimId);
        }
    }
}
