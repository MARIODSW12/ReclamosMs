using Reclamos.Domain.Aggregates;

namespace Reclamos.Domain.Repositories
{
    public interface IClaimRepository
    {
        Task CreateClaim(Claim claim);
        Task SolveClaim(Claim claim);
        Task<Claim?> GetClaimById(string id);
        Task OpenClaim(Claim claim);
    }
}
