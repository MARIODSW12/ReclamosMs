using MongoDB.Bson;
using MongoDB.Driver;

using Reclamos.Infrastructure.Configurations;
using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Persistences.Repositories.MongoRead
{
    public class ClaimReadRepository: IClaimReadRepository
    {
        private readonly IMongoCollection<BsonDocument> ReclamoColexion;

        public ClaimReadRepository(MongoReadReclamoDbConfig mongoConfig)
        {
            ReclamoColexion = mongoConfig.db.GetCollection<BsonDocument>("reclamos_read");
        }

        #region GetTodosLosReclamos()
        public async Task<List<BsonDocument>> GetTodosLosReclamosPorStatus(string status)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("status", status);
                var reclamos = await ReclamoColexion.Find(filter).ToListAsync();

                if (reclamos == null || !reclamos.Any())
                {
                    return new List<BsonDocument>();
                }

                return reclamos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region GetReclamoPorId(string idReclamo)
        public async Task<BsonDocument> GetReclamoPorId(string idReclamo)
        {
            try
            {
                var filtroIdReclamo = Builders<BsonDocument>.Filter.Eq("_id", idReclamo);

                var reclamo = await ReclamoColexion.Find(filtroIdReclamo).FirstOrDefaultAsync();

                return reclamo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region AgregarReclamo(BsonDocument reclamo)
        public async Task AgregarReclamo(BsonDocument reclamo)
        {
            try
            {
                await ReclamoColexion.InsertOneAsync(reclamo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region ActualizarReclamo(BsonDocument reclamo)
        public async Task ResolverReclamo(BsonDocument reclamo)
        {
            try
            {
                var idReclamoActualizar = Builders<BsonDocument>.Filter.Eq("_id", (reclamo["_id"]).AsString);

                var ReclamoActualizar = Builders<BsonDocument>.Update
                    .Set("solution", reclamo["solution"].AsString)
                    .Set("solutionDetail", reclamo["solutionDetail"].AsString)
                    .Set("status", reclamo["status"].AsString);

                var reclamoModificado = await ReclamoColexion.UpdateOneAsync(idReclamoActualizar, ReclamoActualizar);
                
                if (reclamoModificado.ModifiedCount == 0)
                {
                    throw new Exception("No se pudo actualizar el reclamo. Verifique el ID del reclamo.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region AbrirReclamo(BsonDocument reclamo)
        public async Task AbrirReclamo(string id)
        {
            try
            {
                var idReclamoActualizar = Builders<BsonDocument>.Filter.Eq("_id", id);

                var ReclamoActualizar = Builders<BsonDocument>.Update
                    .Set("status", "opened");

                var reclamoModificado = await ReclamoColexion.UpdateOneAsync(idReclamoActualizar, ReclamoActualizar);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

    }
}
