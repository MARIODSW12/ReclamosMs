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
    public class CommandHandler_OpenClaim_tests
    {
        private readonly Mock<IPublishEndpoint> publishEndpointMock;
        private readonly Mock<IClaimRepository> claimRepositoryMock;
        private readonly OpenClaimCommandHandler handler;

        private Claim responseClaim = new Claim(new VOId("9a2e844b-fc27-4b94-949b-9757a3557411"),
            new VOUserId("9a2e844b-fc27-4b94-949b-9757a3557491"),
            new VOAuctionId("9a2e844b-fc27-4b94-949b-9757a3557511"), new VOMotive("Motivo"),
            new VODescription("Descripcion"),
            [new VOEvidence("Evidencia")], new VOStatus("pending"), null, null, null);

        public CommandHandler_OpenClaim_tests()
        {
            claimRepositoryMock = new Mock<IClaimRepository>();
            publishEndpointMock = new Mock<IPublishEndpoint>();
            handler = new OpenClaimCommandHandler( claimRepositoryMock.Object, publishEndpointMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldOpenClaimAndPublishEvent()
        {

            claimRepositoryMock.Setup(repo => repo.OpenClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new OpenClaimCommand("9a2e844b-fc27-4b94-949b-9757a3557411");
            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            claimRepositoryMock.Verify(repo => repo.OpenClaim(It.IsAny<Claim>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<ClaimOpenedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnClaimId_WhenClaimIsOpend()
        {



            claimRepositoryMock.Setup(repo => repo.OpenClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new OpenClaimCommand("9a2e844b-fc27-4b94-949b-9757a3557411");


            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(Guid.TryParse(result, out _));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            claimRepositoryMock.Setup(repo => repo.OpenClaim(It.IsAny<Claim>()))
                .ThrowsAsync(new Exception("Database error"));
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>())).ReturnsAsync(responseClaim);

            var command = new OpenClaimCommand("9a2e844b-fc27-4b94-949b-9757a3557411");


            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenInvalidClaimId()
        {
            claimRepositoryMock.Setup(repo => repo.OpenClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            claimRepositoryMock.Setup(repo => repo.GetClaimById(It.IsAny<string>()))
                .ReturnsAsync((Claim?)null);

            var command = new OpenClaimCommand("9a2e844b-fc27-4b94-949b-9757a3557411");


            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

    }
}
