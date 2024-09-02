using Application.News.Queries.GetAllNews;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.News;

public class GetAllNewsQueryHandlerTests
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly GetAllNewsQueryHandler _handler;

    public GetAllNewsQueryHandlerTests()
    {
        _newsRepositoryMock = new Mock<INewsRepository>();
        _handler = new GetAllNewsQueryHandler(_newsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewsList_WhenNewsExist()
    {
        // Arrange
        var newsList = new List<Domain.Entities.News>
        {
            Domain.Entities.News.Create("Title1", Content.Create("Content1").Value, DateTime.Now, Guid.NewGuid()),
            Domain.Entities.News.Create("Title2", Content.Create("Content2").Value, DateTime.Now, Guid.NewGuid())
        };

        _newsRepositoryMock.Setup(repo => repo.GetAllNewsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(newsList);

        var query = new GetAllNewsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal("Title1", result.Value.First().Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoNewsExist()
    {
        // Arrange
        _newsRepositoryMock.Setup(repo => repo.GetAllNewsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.News>());

        var query = new GetAllNewsQuery(1, 10);

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
        var newsList = new List<Domain.Entities.News>
        {
            Domain.Entities.News.Create("Title1", Content.Create("Content1").Value, DateTime.Now, Guid.NewGuid()),
            Domain.Entities.News.Create("Title2", Content.Create("Content2").Value, DateTime.Now, Guid.NewGuid())
        };

        _newsRepositoryMock.Setup(repo => repo.GetAllNewsAsync(2, 5)) // Page 2, 5 items per page
            .ReturnsAsync(newsList);

        var query = new GetAllNewsQuery(2, 5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _newsRepositoryMock.Verify(repo => repo.GetAllNewsAsync(2, 5), Times.Once);
    }
}
