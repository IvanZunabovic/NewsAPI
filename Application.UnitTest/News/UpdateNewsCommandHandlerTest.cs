using Application.News.Commands.UpdateNews;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.News;
public class UpdateNewsCommandHandlerTests
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly UpdateNewsCommandHandler _handler;

    public UpdateNewsCommandHandlerTests()
    {
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new UpdateNewsCommandHandler(_newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateNews_WhenAllConditionsAreMet()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new UpdateNewsCommand(
            newsId,
            "Updated Title",
            "Updated Content",
            authorId);

        var news = Domain.Entities.News.Create("Original Title", Content.Create("Original Content").Value, DateTime.Now, authorId);
        _newsRepositoryMock.Setup(repo => repo.GetNewsByIdAsync(newsId))
            .ReturnsAsync(news);

        _newsRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", news.Title);
        Assert.Equal("Updated Content", news.Content.Value);
        _newsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenContentIsInvalid()
    {
        // Arrange
        var command = new UpdateNewsCommand(
            Guid.NewGuid(),
            "Updated Title",
            "", // Invalid content
            Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Content.Empty", result.Error.Code);
        _newsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNewsNotFound()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var command = new UpdateNewsCommand(
            Guid.NewGuid(),
            "Updated Title",
            "Updated Content",
            Guid.NewGuid());

        _newsRepositoryMock.Setup(repo => repo.GetNewsByIdAsync(newsId))
            .ReturnsAsync((Domain.Entities.News)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("News.NotFound", result.Error.Code);
        _newsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCurrentUserIsNotAuthor()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new UpdateNewsCommand(
            newsId,
            "Updated Title",
            "Updated Content",
            Guid.NewGuid() // Different author
            );

        var news = Domain.Entities.News.Create("Original Title", Content.Create("Original Content").Value, DateTime.Now, authorId);
        _newsRepositoryMock.Setup(repo => repo.GetNewsByIdAsync(newsId))
            .ReturnsAsync(news);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("News.CantEdit", result.Error.Code);
        _newsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
