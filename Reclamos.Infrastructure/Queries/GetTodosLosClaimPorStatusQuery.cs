using MediatR;

using Reclamos.Application.DTOs;

namespace Reclamos.Infrastructure.Queries
{
    public class GetTodosLosClaimPorStatusQuery: IRequest<List<ClaimDto>>
    {
        public string Status { get; set; }

        public GetTodosLosClaimPorStatusQuery(string status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }
    }
}
