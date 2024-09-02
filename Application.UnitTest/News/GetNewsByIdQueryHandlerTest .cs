using Application.News.Queries.GetNewsById;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.News;
public class GetNewsByIdQueryHandlerTests
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly GetNewsByIdQueryHandler _handler;

    public GetNewsByIdQueryHandlerTests()
    {
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new GetNewsByIdQueryHandler(_newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNews_WhenNewsExists()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var news = Domain.Entities.News.Create("Title", Content.Create("Content").Value, DateTime.Now, Guid.NewGuid());
        news.Id = newsId;
        _newsRepositoryMock.Setup(repo => repo.GetNewsByIdAsync(newsId)).ReturnsAsync(news);

        var query = new GetNewsByIdQuery(newsId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newsId, result.Value.Id);
        Assert.Equal("Title", result.Value.Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNewsNotFound()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        _newsRepositoryMock.Setup(repo => repo.GetNewsByIdAsync(newsId)).ReturnsAsync((Domain.Entities.News)null);

        var query = new GetNewsByIdQuery(newsId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("News.NorFound", result.Error.Code);
        Assert.Equal($"News with Id {newsId} was not found", result.Error.Message);
    }
}
