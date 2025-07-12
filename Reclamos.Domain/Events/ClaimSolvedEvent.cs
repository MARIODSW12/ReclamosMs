using MediatR;

namespace Reclamos.Domain.Events
{
    public class ClaimSolvedEvent : INotification
    {
        public string ClaimId { get; }
        public string Solution { get; }
        public string SolutionDetail { get; }
        public ClaimSolvedEvent(string claimId, string solution, string solutionDetail)
        {
            ClaimId = claimId;
            Solution = solution;
            SolutionDetail = solutionDetail;
        }
    }
}
