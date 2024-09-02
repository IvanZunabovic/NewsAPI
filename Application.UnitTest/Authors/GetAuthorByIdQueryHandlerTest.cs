using Application.Authors.Queries.GetAuthorById;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.Authors;

public class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly GetAuthorByIdQueryHandler _handler;

    public GetAuthorByIdQueryHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _handler = new GetAuthorByIdQueryHandler(_authorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthor_WhenAuthorExists()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var author = Author.Create(
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value,
            "john_doe",
            Email.Create("john.doe@example.com").Value,
            "hashedpassword");
        author.Id = authorId;

        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId))
            .ReturnsAsync(author);

        var query = new GetAuthorByIdQuery(authorId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(authorId, result.Value.Id);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("john_doe", result.Value.UserName);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAuthorNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId))
            .ReturnsAsync((Author)null);

        var query = new GetAuthorByIdQuery(authorId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author.NotFound", result.Error.Code);
        Assert.Equal($"Author with Id {authorId} was not found", result.Error.Message);
    }
}
