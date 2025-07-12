using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Moq;
using Reclamos.Application.Commands;
using Reclamos.Application.DTOs;
using Reclamos.Application.Handlers;
using Reclamos.Domain.Aggregates;
using Reclamos.Domain.Events;
using Reclamos.Domain.Exceptions;
using Reclamos.Domain.Repositories;
using Reclamos.Domain.ValueObjects;

namespace Reclamos.Tests.Reclamos.Application.commands
{
    public class CommandHandler_SolveClaim_tests
    {
        private readonly Mock<IPublishEndpoint> publishEndpointMock;
        private readonly Mock<IClaimRepository> claimRepositoryMock;
        private readonly SolveClaimCommandHandler handler;

        private Claim responseClaim = new Claim(new VOId("9a2e844b-fc27-4b94-949b-9757a3557411"),
            new VOUserId("9a2e844b-fc27-4b94-949b-9757a3557491"),
            new VOAuctionId("9a2e844b-fc27-4b94-949b-9757a3557511"), new VOMotive("Motivo"),
            new VODescription("Descripcion"),
            [new VOEvidence("Evidencia")], new VOStatus("pending"), null, null, null);

        public CommandHandler_SolveClaim_tests()
        {
            claimRepositoryMock = new Mock<IClaimRepository>();
            publishEndpointMock = new Mock<IPublishEndpoint>();
            handler = new SolveClaimCommandHandler( claimRepositoryMock.Object, publishEndpointMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldSolveClaimAndPublishEvent()
        {

            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                Solution = "Test Solution",
                SolutionDetail = "Test Solution Detail",
            });
            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            claimRepositoryMock.Verify(repo => repo.SolveClaim(It.IsAny<Claim>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<ClaimSolvedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnClaimId_WhenClaimIsSolved()
        {



            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                Solution = "Test Solution",
                SolutionDetail = "Test Solution Detail",
            });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(Guid.TryParse(result, out _));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .ThrowsAsync(new Exception("Database error"));
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                Solution = "Test Solution",
                SolutionDetail = "Test Solution Detail",
            });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenInvalidClaimId()
        {
            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>()))
                .ReturnsAsync((Claim?)null);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "id",
                Solution = "Test Solution",
                SolutionDetail = "Test Solution Detail",
            });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenVoidSolution()
        {
            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                Solution = null,
                SolutionDetail = "Test Solution Detail",
            });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidSolutionException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenVoidSolutionDetail()
        {
            claimRepositoryMock.Setup(repo => repo.SolveClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new SolveClaimCommand(new SolveClaimDto
            {
                ClaimId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                Solution = "Test Solution",
                SolutionDetail = null,
            });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidSolutionDetailException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
