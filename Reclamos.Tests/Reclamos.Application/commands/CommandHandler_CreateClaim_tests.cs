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

namespace Reclamos.Tests.Reclamos.Application.commands
{
    public class CommandHandler_CreateClaim_tests
    {
        private readonly Mock<IPublishEndpoint> publishEndpointMock;
        private readonly Mock<IClaimRepository> claimRepositoryMock;
        private readonly CreateClaimCommandHandler handler;

        public CommandHandler_CreateClaim_tests()
        {
            claimRepositoryMock = new Mock<IClaimRepository>();
            publishEndpointMock = new Mock<IPublishEndpoint>();
            handler = new CreateClaimCommandHandler(claimRepositoryMock.Object, publishEndpointMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateClaimAndPublishEvent()
        {

            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);

            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                Motive = "Test Motive",
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);
               

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            claimRepositoryMock.Verify(repo => repo.CreateClaim(It.IsAny<Claim>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<ClaimCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnClaimId_WhenClaimIsCreated()
        {
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);

            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                Motive = "Test Motive",
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(Guid.TryParse(result, out _));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .ThrowsAsync(new Exception("Database error"));

            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                Motive = "Test Motive",
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenInvalidUserId()
        {
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "id",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                Motive = "Test Motive",
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUserIdException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenInvalidAuctionId()
        {
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                AuctionId = "id",
                Motive = "Test Motive",
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidAuctionIdException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenVoidMotive()
        {
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557414",
                Motive = null,
                Description = "Test Description"
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMotiveException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenVoidDescription()
        {
            claimRepositoryMock.Setup(repo => repo.CreateClaim(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask);
            var command = new CreateClaimCommand(new CreateClaimDto
            {
                UserId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557414",
                Motive = "Test Motive",
                Description = null
            }, new List<string> { "evidence1", "evidence2" }, null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDescriptionException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
