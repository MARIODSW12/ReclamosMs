using MediatR;

using Reclamos.Application.DTOs;

namespace Reclamos.Infrastructure.Queries
{
    public class GetClaimPorIdQuery : IRequest<ClaimDto>
    {
        public string Id { get; set; }
        public GetClaimPorIdQuery(string id)
        {
            Id = id;
        }
    }
}
