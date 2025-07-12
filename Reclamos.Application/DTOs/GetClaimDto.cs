
namespace Reclamos.Application.DTOs
{
    public class ClaimDto
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required string AuctionId { get; init; }
        public string? PrizeClaimId { get; init; }
        public required string Motive { get; init; }
        public required string Description { get; init; }
        public required string Status { get; init; }
        public required List<string> Evidence { get; init; }
        public string? Solution { get; init; }
        public string? SolutionDetail { get; init; }
    }
}
