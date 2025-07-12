using System.Runtime.CompilerServices;
using MediatR;
using MongoDB.Bson;
using Reclamos.Application.DTOs;

using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Queries.QueryHandler
{
    public class GetTodosLosClaimPorStatusQueryHandler : IRequestHandler<GetTodosLosClaimPorStatusQuery, List<ClaimDto>>
    {
        private readonly IClaimReadRepository ClaimRepository;

        public GetTodosLosClaimPorStatusQueryHandler(IClaimReadRepository claimRepository)
        {
            ClaimRepository = claimRepository;
        }

        public async Task<List<ClaimDto>> Handle(GetTodosLosClaimPorStatusQuery todosClaims, CancellationToken cancellationToken)
        {
            try
            {
                var claims = await ClaimRepository.GetTodosLosReclamosPorStatus(todosClaims.Status);

                if (claims == null || !claims.Any())
                {
                    return new List<ClaimDto>();
                }

                var listaClaims = claims.Select(claim => new ClaimDto
                {
                    Id = claim["_id"].AsString,
                    UserId = claim["userId"].AsString,
                    AuctionId = claim["auctionId"].AsString,
                    Motive = claim["motive"].AsString,
                    Description = claim["description"].AsString,
                    Status = claim["status"].AsString,
                    Solution = claim["solution"] != BsonNull.Value ? claim["solution"].AsString : null,
                    SolutionDetail = claim["solutionDetail"] != BsonNull.Value ? claim["solutionDetail"].AsString : null,
                    Evidence = claim["evidence"].AsBsonArray.Select(e => e.AsString).ToList(),
                    PrizeClaimId = claim["prizeClaimId"] != BsonNull.Value ? claim["prizeClaimId"].AsString : null
                }).ToList();

                return listaClaims;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
