using MongoDB.Bson;

namespace Reclamos.Infrastructure.Interfaces
{
    public interface IClaimReadRepository
    {
        Task<List<BsonDocument>> GetTodosLosReclamosPorStatus(string status);
        Task<BsonDocument> GetReclamoPorId(string idReclamo);
        Task AgregarReclamo(BsonDocument reclamo);
        Task ResolverReclamo(BsonDocument reclamo);
        Task AbrirReclamo(string id);
    }
}
