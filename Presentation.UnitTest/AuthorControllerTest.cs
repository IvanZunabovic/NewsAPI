using Application.Authors.Commands.UpdateAuthorInfo;
using Application.Authors.Queries.GetAllAuthors;
using Application.Authors.Queries.GetAuthorById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Domain.Shared;
using Application.Authors.Queries.ResponseModels;

namespace Presentation.UnitTest;
public class AuthorControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly AuthorController _controller;

    public AuthorControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new AuthorController(_senderMock.Object);
    }

    [Fact]
    public async Task GetAuthorById_ReturnsOkResult_WhenAuthorExists()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var authorResponse = new AuthorResponse(authorId, "John", "Doe", "john.doe@example.com", "johndoe");

        _senderMock.Setup(s => s.Send(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(authorResponse));

        // Act
        var result = await _controller.GetAuthorById(authorId, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(authorResponse, result.Value);
    }

    [Fact]
    public async Task GetAuthorById_ReturnsNotFound_WhenAuthorDoesNotExist()
    {
        // Arrange
        var authorId = Guid.NewGuid();

        _senderMock.Setup(s => s.Send(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthorResponse>(new Error("Author.NotFound", "Author not found")));

        // Act
        var result = await _controller.GetAuthorById(authorId, CancellationToken.None) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetAllAuthors_ReturnsOkResult_WithAuthorsList()
    {
        // Arrange
        var authors = new List<AuthorResponse>
        {
            new AuthorResponse(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "johndoe")
        };

        _senderMock.Setup(s => s.Send(It.IsAny<GetAllAuthorsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<AuthorResponse>>(authors));

        // Act
        var result = await _controller.GetAllAuthors(1, 10, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(authors, result.Value);
    }

    [Fact]
    public async Task UpdateAuthorInfo_ReturnsOk_WhenUpdateSuccessful()
    {
        // Arrange
        var command = new UpdateAuthorInfoCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe");

        _senderMock.Setup(s => s.Send(It.IsAny<UpdateAuthorInfoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.UpdateAuthorInfo(command, CancellationToken.None) as OkResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthorInfo_ReturnsBadRequest_WhenUpdateFails()
    {
        // Arrange
        var command = new UpdateAuthorInfoCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe");

        _senderMock.Setup(s => s.Send(It.IsAny<UpdateAuthorInfoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("Update.Failed", "Update failed")));

        // Act
        var result = await _controller.UpdateAuthorInfo(command, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}
