using MongoDB.Bson;
using Moq;
using Reclamos.Application.DTOs;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Infrastructure.Queries;
using Reclamos.Infrastructure.Queries.QueryHandler;


namespace Reclamos.Tests.Reclamos.Infrastructure.Queries
{
    public class GetClaimPorIdQueryHandlerTests
    {
        private readonly Mock<IClaimReadRepository> ClaimReadRepositoryMock;
        private readonly GetClaimPorIdQueryHandler Handler;

        private BsonDocument claim1 = new BsonDocument
        {
            { "solution", BsonNull.Value },
            { "solutionDetail", BsonNull.Value },
            { "evidence", new BsonArray() },
            { "prizeClaimId", BsonNull.Value },
            { "_id", "9a2e844b-fc27-4b94-949b-9757a3557411" },
            { "userId", "9a2e844b-fc27-4b94-949b-9757a3557511" },
            { "auctionId", "9a2e844b-fc27-4b94-949b-9757a3557611" },
            { "motive", "Motivo" },
            { "description", "Descripcion" },
            { "status", "pending" }
            
        };

        private ClaimDto claimDto1 = new ClaimDto
        {
            Solution = null,
            SolutionDetail = null,
            Evidence = new List<string>(),
            PrizeClaimId = null,
            Id = "9a2e844b-fc27-4b94-949b-9757a3557411",
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557511",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557611",
            Motive = "Motivo",
            Description = "Descripcion",
            Status = "pending"
        };
        public GetClaimPorIdQueryHandlerTests()
        {
            ClaimReadRepositoryMock = new Mock<IClaimReadRepository>();
            Handler = new GetClaimPorIdQueryHandler(ClaimReadRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsClaim_WhenClaimExist()
        {
            var query = new GetClaimPorIdQuery("9a2e844b-fc27-4b94-949b-9757a3557411");
            
            ClaimReadRepositoryMock.Setup(r => r.GetReclamoPorId(query.Id))
                .ReturnsAsync(claim1);

            var result = await Handler.Handle(query, CancellationToken.None);

            Assert.Equal(claimDto1.Id, result.Id);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenClaimDoesNotExist()
        {
            var query = new GetClaimPorIdQuery("non-existing-id");

            ClaimReadRepositoryMock.Setup(r => r.GetReclamoPorId(query.Id))
                .ReturnsAsync((BsonDocument)null);

            var result = await Handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            var query = new GetClaimPorIdQuery("9a2e844b-fc27-4b94-949b-9757a3557411");

            ClaimReadRepositoryMock.Setup(r => r.GetReclamoPorId(query.Id))
                .ThrowsAsync(new Exception("Database error"));

            await Assert.ThrowsAsync<Exception>(() => Handler.Handle(query, CancellationToken.None));
        }

    }
}
