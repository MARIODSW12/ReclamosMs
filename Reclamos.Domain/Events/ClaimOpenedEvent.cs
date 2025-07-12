using MediatR;

namespace Reclamos.Domain.Events
{
    public class ClaimOpenedEvent : INotification
    {
        public string ClaimId { get; }
        public ClaimOpenedEvent(string claimId)
        {
            ClaimId = claimId;
        }
    }
}
