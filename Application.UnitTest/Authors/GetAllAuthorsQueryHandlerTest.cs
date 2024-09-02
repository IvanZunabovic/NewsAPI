using Application.Authors.Queries.GetAllAuthors;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.Authors;
public class GetAllAuthorsQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly GetAllAuthorsQueryHandler _handler;

    public GetAllAuthorsQueryHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _handler = new GetAllAuthorsQueryHandler(_authorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthors_WhenAuthorsExist()
    {
        // Arrange
        var authors = new List<Author>
        {
            Author.Create(FirstName.Create("John").Value, LastName.Create("Doe").Value, "john_doe", Email.Create("john.doe@example.com").Value, "hashedpassword"),
            Author.Create(FirstName.Create("Jane").Value, LastName.Create("Smith").Value, "jane_smith", Email.Create("jane.smith@example.com").Value, "hashedpassword")
        };

        _authorRepositoryMock.Setup(repo => repo.GetAllAuthorsAsync(1, 10)).ReturnsAsync(authors);

        var query = new GetAllAuthorsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal("John", result.Value.First().FirstName);
        Assert.Equal("Jane", result.Value.Last().FirstName);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoAuthorsExist()
    {
        // Arrange
        var authors = new List<Author>();
        _authorRepositoryMock.Setup(repo => repo.GetAllAuthorsAsync(1, 10)).ReturnsAsync(authors);

        var query = new GetAllAuthorsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
