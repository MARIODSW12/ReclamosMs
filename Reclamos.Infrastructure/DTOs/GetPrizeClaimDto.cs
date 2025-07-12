

namespace Reclamos.Application.DTOs
{
    public class GetPrizeClaimDto
    {
        public string id { get; init; }
        public string userId { get; init; }
        public string auctionId { get; init; }
        public string deliverDirection { get; init; }
        public string deliverMethod { get; init; }
        public DateTime claimDate { get; init; }
    }
}
