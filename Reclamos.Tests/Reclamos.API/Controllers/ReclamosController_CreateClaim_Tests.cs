using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Reclamos.Application.Commands;
using Reclamos.Application.DTOs;
using Reclamos.Domain.Exceptions;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Presentation.Controllers;
using RestSharp;

namespace Reclamos.Tests.Reclamos.API.Controllers;

public class ReclamosController_CreateClaim_Tests
{
    private readonly Mock<IMediator> MediatorMock;
    private readonly Mock<IPublishEndpoint> PublishEndpointMock;
    private readonly Mock<ICloudinaryService> CloudinaryServiceMock;
    private readonly Mock<IRestClient> RestClientMock;
    private readonly ClaimController Controller;

    public ReclamosController_CreateClaim_Tests()
    {
        MediatorMock = new Mock<IMediator>();
        PublishEndpointMock = new Mock<IPublishEndpoint>();
        CloudinaryServiceMock = new Mock<ICloudinaryService>();
        RestClientMock = new Mock<IRestClient>();
        Controller = new ClaimController(MediatorMock.Object, PublishEndpointMock.Object, CloudinaryServiceMock.Object, RestClientMock.Object);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnOk_WhenClaimIsCreatedSuccessfully()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("9a2e844b-fc27-4b94-949b-9757a3557415");

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        var okResult = result as CreatedAtActionResult;
        Assert.Equal((new { id = "9a2e844b-fc27-4b94-949b-9757a3557415" }).ToString(), okResult.Value.ToString());
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnBadRequest_WhenClaimCreationFails()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnBadRequest_WhenInvalidInput()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "invalid-id",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidUserIdException());

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnBadRequest_WhenInvalidAuctionId()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "invalid-id",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidAuctionIdException());

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnBadRequest_WhenInvalidEvidence()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        var evidences = new List<IFormFile>();

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidEvidenceException());

        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaim_ShouldReturnBadRequest_WhenCloudinaryFails()
    {
        // Arrange
        var claimDto = new CreateClaimDto
        {
            UserId = "9a2e844b-fc27-4b94-949b-9757a3557411",
            AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
            Motive = "Test Motive",
            Description = "Test Description"
        };
        var textEvidence = "Test Evidence";
        List<IFormFile> evidences = [new FormFile(Stream.Null, 2, 10, "a", "a")];

        CloudinaryServiceMock.Setup(s => s.SubirArchivo(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Cloudinary upload failed"));
        // Act
        var result = await Controller.CreateClaim(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }
}
