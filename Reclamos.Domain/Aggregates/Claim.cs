using Reclamos.Domain.Exceptions;
using Reclamos.Domain.ValueObjects;

namespace Reclamos.Domain.Aggregates
{
    public class Claim
    {
        public VOId Id { get; private set; }
        public VOUserId UserId { get; private set; }
        public VOAuctionId AuctionId { get; private set; }
        public VOPrizeClaimId? PrizeClaimId { get; private set; }
        public VOMotive Motive {  get; private set; }
        public VODescription Description { get; private set; }
        public List<VOEvidence> Evidence { get; private set; }
        public VOStatus Status { get; private set; }
        public VOSolution? Solution { get; private set; }
        public VOSolutionDetail? SolutionDetail { get; private set; }


        public Claim(VOId id, VOUserId userId, VOAuctionId auctionId, VOMotive motive,
            VODescription description, List<VOEvidence> evidence, VOStatus status, VOSolution? solution = null,
            VOSolutionDetail? solutionDetail = null, VOPrizeClaimId? prizeClaimId = null)
        {
            Id = id;
            UserId = userId;
            AuctionId = auctionId;
            Motive = motive;
            Description = description;
            Evidence = evidence ?? new List<VOEvidence>();
            Solution = solution;
            SolutionDetail = solutionDetail;
            Status = status;
            PrizeClaimId = prizeClaimId;
        }
        public void SolveClaim(string solution, string solutionDetail)
        {
            this.Solution = new VOSolution(solution);
            this.SolutionDetail = new VOSolutionDetail(solutionDetail);
            this.Status = new VOStatus("solved");
        }

        public void Openclaim()
        {
            this.Status = new VOStatus("opened");
        }

    }
}