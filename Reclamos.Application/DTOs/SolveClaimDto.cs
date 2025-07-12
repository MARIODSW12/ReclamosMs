
namespace Reclamos.Application.DTOs
{
    public class SolveClaimDto
    {
        public required string ClaimId { get; init; } 
        public required string Solution { get; init; }
        public required string SolutionDetail { get; init; }
    }
}
