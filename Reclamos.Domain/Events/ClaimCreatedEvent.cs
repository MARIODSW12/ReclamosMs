using MediatR;

namespace Reclamos.Domain.Events
{
    public class ClaimCreatedEvent : INotification
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string AuctionId { get; private set; }
        public string Motive { get; private set; }
        public string Description { get; private set; }
        public List<string> Evidence { get; private set; }
        public string Status { get; private set; }
        public string? PrizeClaimId { get; private set; }

        public ClaimCreatedEvent(string id, string userId, string auctionId, string motive,
            string description, List<string> evidence, string status, string? prizeClaimId)
        {
            Id = id;
            UserId = userId;
            AuctionId = auctionId;
            Motive = motive;
            Description = description;
            Evidence = evidence;
            Status = status;
            PrizeClaimId = prizeClaimId;
        }
    }
}
