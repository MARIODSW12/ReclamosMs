using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Reclamos.Domain.Aggregates;
using Reclamos.Domain.ValueObjects;
using Reclamos.Infrastructure.Configurations;
using Reclamos.Infrastructure.Persistences.Repositories.MongoWrite;

namespace Reclamos.Tests.Reclamos.Infrastructure.Persistence.Repository.MongoWrite
{
    public class MongoWriteUserRepositoryTests
    {
        private readonly Mock<IMongoDatabase> mongoDatabaseMock;
        private readonly Mock<IMongoCollection<BsonDocument>> claimCollectionMock;
        private readonly ClaimWriteRepository repository;

        private BsonDocument claim1 = new BsonDocument
        {
            { "_id", "9a2e844b-fc27-4b94-949b-9757a3557411" },
            { "userId", "9a2e844b-fc27-4b94-949b-9757a3557511" },
            { "auctionId", "9a2e844b-fc27-4b94-949b-9757a3557611" },
            { "motive", "Motivo" },
            { "description", "Descripcion" },
            { "evidence", new BsonArray() },
            { "status", "pending" },
            { "solution", "Test solution" },
            { "solutionDetail", "Solution detail test" },
            { "prizeClaimId", BsonNull.Value }
        };
        private Claim domainClaim = new Claim(new VOId("9a2e844b-fc27-4b94-949b-9757a3557411"),
            new VOUserId("9a2e844b-fc27-4b94-949b-9757a3557511"),
            new VOAuctionId("9a2e844b-fc27-4b94-949b-9757a3557611"), new VOMotive("Motivo"),
            new VODescription("Descripcion"),
            [], new VOStatus("pending"), new VOSolution("Test solution"), new VOSolutionDetail("Solution detail test"), null);
        public MongoWriteUserRepositoryTests()
        {
            mongoDatabaseMock = new Mock<IMongoDatabase>();
            claimCollectionMock = new Mock<IMongoCollection<BsonDocument>>();

            mongoDatabaseMock.Setup(d => d.GetCollection<BsonDocument>("reclamos_write", It.IsAny<MongoCollectionSettings>()))
                              .Returns(claimCollectionMock.Object);

            Environment.SetEnvironmentVariable("MONGODB_CNN_WRITE", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME_WRITE", "test_database");
            var mongoConfigMock = new MongoWriteReclamoDbConfig();
            mongoConfigMock.db = mongoDatabaseMock.Object;

            repository = new ClaimWriteRepository(mongoConfigMock);
        }

        [Fact]
        public async Task GetReclamoPorId_GetsClaims()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([claim1]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ReturnsAsync(cursorMock.Object);

            var result = await repository.GetClaimById("9a2e844b-fc27-4b94-949b-9757a3557411");

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
            Assert.Equal(domainClaim.Id.Value, result.Id.Value);
        }

        [Fact]
        public async Task GetReclamoPorId_GetsNull()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ReturnsAsync(cursorMock.Object);

            var result = await repository.GetClaimById("9a2e844b-fc27-4b94-949b-9757a3557411");

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task GetReclamoPorId_ThrowsError()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ThrowsAsync(new Exception("database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.GetClaimById("9a2e844b-fc27-4b94-949b-9757a3557411"));

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
        }

        [Fact]
        public async Task AgregarReclamo_CreatesClaim()
        {

            claimCollectionMock.Setup(c => c.InsertOneAsync(claim1, null, default)).Returns(Task.CompletedTask);

            await repository.CreateClaim(domainClaim);

            claimCollectionMock.Verify(c => c.InsertOneAsync(claim1, null, default), Times.Once);
        }

        [Fact]
        public async Task AgregarReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.InsertOneAsync(claim1, null, default)).ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.CreateClaim(domainClaim));

            claimCollectionMock.Verify(c => c.InsertOneAsync(claim1, null, default), Times.Once);
        }

        [Fact]
        public async Task ResolverReclamo_solvesClaim()
        {
            var updateResultMock = new UpdateResult.Acknowledged(1, 1, claim1["_id"]);
            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ReturnsAsync(updateResultMock);

            await repository.SolveClaim(domainClaim);

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task ResolverReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.SolveClaim(domainClaim));
         
            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task AbrirReclamo_opensClaim()
        {
            var updateResultMock = new UpdateResult.Acknowledged(1, 1, claim1["_id"]);
            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ReturnsAsync(updateResultMock);

            await repository.OpenClaim(domainClaim);

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task AbrirReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.OpenClaim(domainClaim));

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

    }
}
