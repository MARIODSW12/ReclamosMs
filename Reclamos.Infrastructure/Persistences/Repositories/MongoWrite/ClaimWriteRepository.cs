using MongoDB.Bson;
using MongoDB.Driver;

using Reclamos.Domain.Aggregates;
using Reclamos.Domain.Exceptions;
using Reclamos.Domain.Factories;
using Reclamos.Domain.Repositories;
using Reclamos.Domain.ValueObjects;
using Reclamos.Infrastructure.Configurations;

namespace Reclamos.Infrastructure.Persistences.Repositories.MongoWrite
{
    public class ClaimWriteRepository: IClaimRepository
    {
        private readonly IMongoCollection<BsonDocument> ReclamoColexion;

        public ClaimWriteRepository(MongoWriteReclamoDbConfig mongoConfig)
        {
            ReclamoColexion = mongoConfig.db.GetCollection<BsonDocument>("reclamos_write");
        }

        #region AgregarReclamo(Claim reclamo)
        public async Task CreateClaim(Claim reclamo)
        {
            try
            {

                var bsonClaim = new BsonDocument
                {
                    { "_id", reclamo.Id.Value},
                    { "userId", reclamo.UserId.Value},
                    { "auctionId", reclamo.AuctionId.Value},
                    { "motive", reclamo.Motive.Value},
                    { "description", reclamo.Description.Value},
                    { "evidence", reclamo.Evidence.Count > 0 ? new BsonArray(reclamo.Evidence.Select(e => e.Value)) : new BsonArray() },
                    { "status", reclamo.Status.Value},
                    { "solution", reclamo.Solution == null ? BsonNull.Value : reclamo.Solution.Value},
                    { "solutionDetail", reclamo.SolutionDetail == null ? BsonNull.Value : reclamo.SolutionDetail.Value },
                    { "prizeClaimId", reclamo.PrizeClaimId == null ? BsonNull.Value : reclamo.PrizeClaimId.Value }
                };

                await ReclamoColexion.InsertOneAsync(bsonClaim);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region ActualizarReclamo(Claim reclamo)
        public async Task SolveClaim(Claim reclamo)
        {
            try
            {
                var idReclamoActualizar = Builders<BsonDocument>.Filter.Eq("_id", reclamo.Id.Value);

                var actualizar = Builders<BsonDocument>.Update
                    .Set("solution", reclamo.Solution!.Value)
                    .Set("solutionDetail", reclamo.SolutionDetail!.Value)
                    .Set("status", reclamo.Status.Value);

                var reclamoModificado = await ReclamoColexion.UpdateOneAsync(idReclamoActualizar, actualizar);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region ObtenerReclamoPorId(string idReclamo)
        public async Task<Claim?> GetClaimById(string idReclamo)
        {
            try
            {
                var idReclamoBuscar = Builders<BsonDocument>.Filter.Eq("_id", idReclamo);
                var reclamoEncontrado = await ReclamoColexion.Find(idReclamoBuscar).FirstOrDefaultAsync();
                if (reclamoEncontrado == null)
                {
                    return null;
                }
                return ClaimFactory.Create(new VOId(reclamoEncontrado["_id"].AsString), new VOUserId(reclamoEncontrado["userId"].AsString),
                    new VOAuctionId(reclamoEncontrado["auctionId"].AsString), new VOMotive(reclamoEncontrado["motive"].AsString), new VODescription(reclamoEncontrado["description"].AsString),
                    reclamoEncontrado["evidence"].AsBsonArray.Select(p => new VOEvidence(p.AsString)).ToList(), new VOStatus(reclamoEncontrado["status"].AsString),
                    reclamoEncontrado["solution"] == BsonNull.Value ? null : new VOSolution(reclamoEncontrado["solution"].AsString),
                    reclamoEncontrado["solutionDetail"] == BsonNull.Value ? null : new VOSolutionDetail(reclamoEncontrado["solutionDetail"].AsString),
                    reclamoEncontrado["prizeClaimId"] == BsonNull.Value ? null : new VOPrizeClaimId(reclamoEncontrado["prizeClaimId"].AsString));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region OpenClaim
        public async Task OpenClaim(Claim reclamo)
        {
            try
            {
                var idReclamoActualizar = Builders<BsonDocument>.Filter.Eq("_id", reclamo.Id.Value);

                var actualizar = Builders<BsonDocument>.Update
                    .Set("status", reclamo.Status.Value);

                var reclamoModificado = await ReclamoColexion.UpdateOneAsync(idReclamoActualizar, actualizar);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
