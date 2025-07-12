
namespace Reclamos.Application.DTOs
{
    public class CreateClaimDto
    {
        public string UserId { get; init; }
        public string AuctionId { get; init; }
        public string Motive { get; init; }
        public string Description { get; init; }
    }
}
