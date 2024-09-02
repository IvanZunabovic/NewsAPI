using Application.News.Queries.GetNewsByAuthor;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.News;

public class GetNewsByAuthorQueryHandlerTests
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly GetNewsByAuthorQueryHandler _handler;

    public GetNewsByAuthorQueryHandlerTests()
    {
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new GetNewsByAuthorQueryHandler(_newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewsList_WhenNewsExistForAuthor()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var newsList = new List<Domain.Entities.News>
        {
            Domain.Entities.News.Create("Title1", Content.Create("Content1").Value, DateTime.Now, authorId),
            Domain.Entities.News.Create("Title2", Content.Create("Content2").Value, DateTime.Now, authorId)
        };

        _newsRepositoryMock.Setup(repo => repo.GetNewsByAuthorAsync(authorId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(newsList);

        var query = new GetNewsByAuthorQuery(authorId, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal("Title1", result.Value.First().Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoNewsExistForAuthor()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        _newsRepositoryMock.Setup(repo => repo.GetNewsByAuthorAsync(authorId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.News>());

        var query = new GetNewsByAuthorQuery(authorId, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldRespectPaginationParameters()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var newsList = new List<Domain.Entities.News>
        {
            Domain.Entities.News.Create("Title1", Content.Create("Content1").Value, DateTime.Now, authorId),
            Domain.Entities.News.Create("Title2", Content.Create("Content2").Value, DateTime.Now, authorId)
        };

        _newsRepositoryMock.Setup(repo => repo.GetNewsByAuthorAsync(authorId, 2, 5)) // Page 2, 5 items per page
            .ReturnsAsync(newsList);

        var query = new GetNewsByAuthorQuery(authorId, 2, 5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _newsRepositoryMock.Verify(repo => repo.GetNewsByAuthorAsync(authorId, 2, 5), Times.Once);
    }
}
