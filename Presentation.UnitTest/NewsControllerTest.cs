using Application.News.Commands.CreateNews;
using Application.News.Commands.DeleteNews;
using Application.News.Commands.UpdateNews;
using Application.News.Queries.GetAllNews;
using Application.News.Queries.GetNewsByAuthor;
using Application.News.Queries.GetNewsById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Presentation.Controllers;
using Domain.Shared;
using Application.News.Queries.ResponseModels;

namespace Presentation.UnitTest;
public class NewsControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new NewsController(_senderMock.Object);
    }

    [Fact]
    public async Task CreateNews_ReturnsOkResult_WhenCreationIsSuccessful()
    {
        // Arrange
        var command = new CreateNewsCommand(
            "New Title",
            "News content",
            Guid.NewGuid());

        _senderMock.Setup(s => s.Send(It.IsAny<CreateNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.CreateNews(command, CancellationToken.None) as OkResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task CreateNews_ReturnsBadRequest_WhenCreationFails()
    {
        // Arrange
        var command = new CreateNewsCommand(
            "New Title",
            "News content",
            Guid.NewGuid());

        _senderMock.Setup(s => s.Send(It.IsAny<CreateNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("CreationFailed", "Failed to create news")));

        // Act
        var result = await _controller.CreateNews(command, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetNewsById_ReturnsOkResult_WhenNewsExists()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var newsResponse = new NewsResponse(newsId, "Title", "Content", DateTime.UtcNow);

        _senderMock.Setup(s => s.Send(It.IsAny<GetNewsByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(newsResponse));

        // Act
        var result = await _controller.GetNewsById(newsId, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(newsResponse, result.Value);
    }

    [Fact]
    public async Task GetNewsById_ReturnsNotFound_WhenNewsDoesNotExist()
    {
        // Arrange
        var newsId = Guid.NewGuid();

        _senderMock.Setup(s => s.Send(It.IsAny<GetNewsByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<NewsResponse>(new Error("News.NotFound", "News not found")));

        // Act
        var result = await _controller.GetNewsById(newsId, CancellationToken.None) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetAllNews_ReturnsOkResult_WithNewsList()
    {
        // Arrange
        var newsList = new List<NewsResponse>
        {
            new NewsResponse(Guid.NewGuid(), "Title", "Content", DateTime.UtcNow)
        };

        _senderMock.Setup(s => s.Send(It.IsAny<GetAllNewsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<NewsResponse>>(newsList));

        // Act
        var result = await _controller.GetAllNews(1, 10, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(newsList, result.Value);
    }

    [Fact]
    public async Task GetNewsByAuthor_ReturnsOkResult_WithNewsList()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var newsList = new List<NewsResponse>
        {
            new NewsResponse(Guid.NewGuid(), "Title", "Content", DateTime.UtcNow)
        };

        _senderMock.Setup(s => s.Send(It.IsAny<GetNewsByAuthorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<NewsResponse>>(newsList));

        // Act
        var result = await _controller.GetNewsByAuthor(authorId, 1, 10, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(newsList, result.Value);
    }

    [Fact]
    public async Task UpdateNews_ReturnsOk_WhenUpdateSuccessful()
    {
        // Arrange
        var command = new UpdateNewsCommand(
            Guid.NewGuid(),
            "Updated Title",
            "Updated content",
            Guid.NewGuid());

        _senderMock.Setup(s => s.Send(It.IsAny<UpdateNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.UpdateNews(command, CancellationToken.None) as OkResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task UpdateNews_ReturnsNotFound_WhenUpdateFails()
    {
        // Arrange
        var command = new UpdateNewsCommand(
            Guid.NewGuid(),
            "Updated Title",
            "Updated content",
            Guid.NewGuid());

        _senderMock.Setup(s => s.Send(It.IsAny<UpdateNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("UpdateFailed", "Update failed")));

        // Act
        var result = await _controller.UpdateNews(command, CancellationToken.None) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task DeleteNews_ReturnsOk_WhenDeletionSuccessful()
    {
        // Arrange
        var newsId = Guid.NewGuid();

        _senderMock.Setup(s => s.Send(It.IsAny<DeleteNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.DeleteNews(newsId, CancellationToken.None) as OkResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task DeleteNews_ReturnsBadRequest_WhenDeletionFails()
    {
        // Arrange
        var newsId = Guid.NewGuid();

        _senderMock.Setup(s => s.Send(It.IsAny<DeleteNewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("DeletionFailed", "Deletion failed")));

        // Act
        var result = await _controller.DeleteNews(newsId, CancellationToken.None) as BadRequestResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}
