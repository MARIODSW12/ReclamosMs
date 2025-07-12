using MediatR;
using MongoDB.Bson;
using Reclamos.Application.DTOs;

using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Queries.QueryHandler
{
    public class GetClaimPorIdQueryHandler : IRequestHandler<GetClaimPorIdQuery, ClaimDto>
    {
        private readonly IClaimReadRepository ClaimRepository;

        public GetClaimPorIdQueryHandler(IClaimReadRepository claimRepository)
        {
            ClaimRepository = claimRepository;
        }

        public async Task<ClaimDto> Handle(GetClaimPorIdQuery idClaim, CancellationToken cancellationToken)
        {
            try
            {
                var claim = await ClaimRepository.GetReclamoPorId(idClaim.Id);

                if (claim == null)
                {
                    return null;
                }

                var claimPorId = new ClaimDto
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
                };

                return claimPorId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
