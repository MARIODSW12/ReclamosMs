using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Reclamos.Infrastructure.Configurations;
using Reclamos.Infrastructure.Persistences.Repositories.MongoRead;

namespace Reclamos.Tests.Reclamos.Infrastructure.Persistence.Repository.MongoRead
{
    public class MongoReadUserRepositoryTests
    {
        private readonly Mock<IMongoDatabase> mongoDatabaseMock;
        private readonly Mock<IMongoCollection<BsonDocument>> claimCollectionMock;
        private readonly ClaimReadRepository repository;

        private BsonDocument claim1 = new BsonDocument
        {
            { "solution", "Test solution" },
            { "solutionDetail", "Solution detail test" },
            { "evidence", new BsonArray() },
            { "prizeClaimId", BsonNull.Value },
            { "_id", "9a2e844b-fc27-4b94-949b-9757a3557411" },
            { "userId", "9a2e844b-fc27-4b94-949b-9757a3557511" },
            { "auctionId", "9a2e844b-fc27-4b94-949b-9757a3557611" },
            { "motive", "Motivo" },
            { "description", "Descripcion" },
            { "status", "pending" }

        };
        public MongoReadUserRepositoryTests()
        {
            mongoDatabaseMock = new Mock<IMongoDatabase>();
            claimCollectionMock = new Mock<IMongoCollection<BsonDocument>>();

            mongoDatabaseMock.Setup(d => d.GetCollection<BsonDocument>("reclamos_read", It.IsAny<MongoCollectionSettings>()))
                              .Returns(claimCollectionMock.Object);

            Environment.SetEnvironmentVariable("MONGODB_CNN_READ", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME_READ", "test_database");
            var mongoConfigMock = new MongoReadReclamoDbConfig();
            mongoConfigMock.db = mongoDatabaseMock.Object;

            repository = new ClaimReadRepository(mongoConfigMock);
        }

        [Fact]
        public async Task GetTodoslosReclamosPorStatus_GetsClaims()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([claim1]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ReturnsAsync(cursorMock.Object);

            var result = await repository.GetTodosLosReclamosPorStatus("pending");

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
            Assert.Equal([claim1], result);
        }

        [Fact]
        public async Task GetTodoslosReclamosPorStatus_GetsemptyArray()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ReturnsAsync(cursorMock.Object);

            var result = await repository.GetTodosLosReclamosPorStatus("pending");

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
            Assert.Equal([], result);
        }

        [Fact]
        public async Task GetTodoslosReclamosPorStatus_ThrowsError()
        {
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns([claim1]);

            claimCollectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default)).ThrowsAsync(new Exception("database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.GetTodosLosReclamosPorStatus("pending"));

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
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

            var result = await repository.GetReclamoPorId("9a2e844b-fc27-4b94-949b-9757a3557411");

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
            Assert.Equal(claim1, result);
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

            var result = await repository.GetReclamoPorId("9a2e844b-fc27-4b94-949b-9757a3557411");

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

            Assert.ThrowsAsync<Exception>(() => repository.GetReclamoPorId("9a2e844b-fc27-4b94-949b-9757a3557411"));

            claimCollectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default), Times.Once);
        }

        [Fact]
        public async Task AgregarReclamo_CreatesClaim()
        {

            claimCollectionMock.Setup(c => c.InsertOneAsync(claim1, null, default)).Returns(Task.CompletedTask);

            await repository.AgregarReclamo(claim1);

            claimCollectionMock.Verify(c => c.InsertOneAsync(claim1, null, default), Times.Once);
        }

        [Fact]
        public async Task AgregarReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.InsertOneAsync(claim1, null, default)).ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.AgregarReclamo(claim1));

            claimCollectionMock.Verify(c => c.InsertOneAsync(claim1, null, default), Times.Once);
        }

        [Fact]
        public async Task ResolverReclamo_solvesClaim()
        {
            var updateResultMock = new UpdateResult.Acknowledged(1, 1, claim1["_id"]);
            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ReturnsAsync(updateResultMock);

            await repository.ResolverReclamo(claim1);

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task ResolverReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.ResolverReclamo(claim1));
         
            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task AbrirReclamo_opensClaim()
        {
            var updateResultMock = new UpdateResult.Acknowledged(1, 1, claim1["_id"]);
            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ReturnsAsync(updateResultMock);

            await repository.AbrirReclamo(claim1["_id"].AsString);

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

        [Fact]
        public async Task AbrirReclamo_ThrowsError()
        {

            claimCollectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ThrowsAsync(new Exception("Database Exception"));

            Assert.ThrowsAsync<Exception>(() => repository.AbrirReclamo(claim1["_id"].AsString));

            claimCollectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default), Times.Once);
        }

    }
}
