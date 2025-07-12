using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Reclamos.Application.Commands;
using Reclamos.Application.DTOs;
using Reclamos.Domain.Exceptions;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Infrastructure.Queries;
using Reclamos.Presentation.Controllers;
using RestSharp;

namespace Reclamos.Tests.Reclamos.API.Controllers;

public class ReclamosController_GetClaimById_Tests
{
    private readonly Mock<IMediator> mediatorMock;
    private readonly Mock<IPublishEndpoint> publishEndpointMock;
    private readonly Mock<ICloudinaryService> cloudinaryServiceMock;
    private readonly Mock<IRestClient> restClientMock;
    private readonly ClaimController controller;

    private ClaimDto claimPorId = new ClaimDto
    {
        Id = "9a2e844b-fc27-4b94-949b-9757a3557411",
        UserId = "9a2e844b-fc27-4b94-949b-9757a3557511",
        AuctionId = "9a2e844b-fc27-4b94-949b-9757a3557611",
        Motive = "Motivo",
        Description = "Descripcion",
        Status = "pending",
        Solution = null,
        SolutionDetail = null,
        Evidence = [],
        PrizeClaimId = null
    };
    public ReclamosController_GetClaimById_Tests()
    {
        mediatorMock = new Mock<IMediator>();
        publishEndpointMock = new Mock<IPublishEndpoint>();
        cloudinaryServiceMock = new Mock<ICloudinaryService>();
        restClientMock = new Mock<IRestClient>();
        controller = new ClaimController( mediatorMock.Object, publishEndpointMock.Object, cloudinaryServiceMock.Object, restClientMock.Object);
    }

    [Fact]
    public async Task GetClaimById_ShouldReturnOk_WhenClaimExists()
    {
        // Arrange
        var claimDto = "9a2e844b-fc27-4b94-949b-9757a3557411";
        mediatorMock.Setup(m => m.Send(It.IsAny<GetClaimPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(claimPorId);

        // Act
        var result = await controller.GetClaimById(claimDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(claimPorId, okResult.Value);
    }

    [Fact]
    public async Task GetClaimById_ShouldReturnNotFound_WhenClaimCreationFails()
    {
        // Arrange
        var claimDto = "9a2e844b-fc27-4b94-949b-9757a3557411";

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClaimPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClaimDto)null);
        // Act
        var result = await controller.GetClaimById(claimDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetClaimById_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var claimDto = "9a2e844b-fc27-4b94-949b-9757a3557411";

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClaimPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));
        // Act
        var result = await controller.GetClaimById(claimDto);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetClaimById_ShouldReturnBadRequest_WhenInvalidInput()
    {
        // Arrange
        var claimDto = "id";

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClaimPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidIdException());
        // Act
        var result = await controller.GetClaimById(claimDto);

        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
    }

}
