using System.Text.Json;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Reclamos.Application.Commands;
using Reclamos.Application.DTOs;
using Reclamos.Domain.Exceptions;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Presentation.Controllers;
using RestSharp;

namespace Reclamos.Tests.Reclamos.API.Controllers;

public class ReclamosController_CreateClaimPrize_Tests
{
    private readonly Mock<IMediator> mediatorMock;
    private readonly Mock<IPublishEndpoint> publishEndpointMock;
    private readonly Mock<ICloudinaryService> cloudinaryServiceMock;
    private readonly Mock<IRestClient> restClientMock;
    private readonly ClaimController controller;

    public ReclamosController_CreateClaimPrize_Tests()
    {
        mediatorMock = new Mock<IMediator>();
        publishEndpointMock = new Mock<IPublishEndpoint>();
        cloudinaryServiceMock = new Mock<ICloudinaryService>();
        restClientMock = new Mock<IRestClient>();
        controller = new ClaimController(mediatorMock.Object, publishEndpointMock.Object, cloudinaryServiceMock.Object, restClientMock.Object);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnOk_WhenClaimIsCreatedSuccessfully()
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

        restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), CancellationToken.None))
            .ReturnsAsync(new RestResponse {IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = System.Net.HttpStatusCode.OK, Content = JsonConvert.SerializeObject(new GetPrizeClaimDto
            {
                auctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                claimDate = new DateTime(),
                deliverDirection = "Test Direction",
                id = "9a2e844b-fc27-4b94-949b-9757a3557415",
                userId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                deliverMethod = "plane"
            })});
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("9a2e844b-fc27-4b94-949b-9757a3557415");

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        var okResult = result as CreatedAtActionResult;
        Assert.Equal((new { id = "9a2e844b-fc27-4b94-949b-9757a3557415" }).ToString(), okResult.Value.ToString());
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenClaimCreationFails()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var okResult = result as ObjectResult;
        Assert.Equal(500, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnInternalServerError_WhenExceptionOccurs()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenInvalidInput()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidUserIdException());

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenInvalidAuctionId()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidAuctionIdException());

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenInvalidEvidence()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidEvidenceException());

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenInvalidStatus()
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

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidStatusException());

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenCloudinaryFails()
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

        cloudinaryServiceMock.Setup(s => s.SubirArchivo(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Cloudinary upload failed"));
        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenCloudinaryGivesInvalidUrl()
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

        cloudinaryServiceMock.Setup(s => s.SubirArchivo(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("");
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidEvidenceException());
        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenNoPrizeFound()
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

        restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), CancellationToken.None))
            .ReturnsAsync(new RestResponse { StatusCode = System.Net.HttpStatusCode.NotFound });

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturn500_WhenResponseDoesNotMatch()
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

        restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), CancellationToken.None))
            .ReturnsAsync(new RestResponse
            {
                IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = "test"
            });
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("9a2e844b-fc27-4b94-949b-9757a3557415");

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var Result = result as ObjectResult;
        Assert.Equal(500, Result.StatusCode);
    }

    [Fact]
    public async Task CreateClaimPrize_ShouldReturnBadRequest_WhenIdIsNull()
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

        restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), CancellationToken.None))
            .ReturnsAsync(new RestResponse
            {
                IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(new GetPrizeClaimDto
                {
                    auctionId = "9a2e844b-fc27-4b94-949b-9757a3557415",
                    claimDate = new DateTime(),
                    deliverDirection = "Test Direction",
                    id = "9a2e844b-fc27-4b94-949b-9757a3557415",
                    userId = "9a2e844b-fc27-4b94-949b-9757a3557411",
                    deliverMethod = "plane"
                })
            });
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateClaimCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);

        // Act
        var result = await controller.CreateClaimPrize(claimDto, textEvidence, evidences);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
