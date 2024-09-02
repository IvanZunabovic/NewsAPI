using Application.News.Commands.CreateNews;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.News;
public class CreateNewsCommandHandlerTests
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly CreateNewsCommandHandler _handler;

    public CreateNewsCommandHandlerTests()
    {
        _newsRepositoryMock = new Mock<INewsRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _handler = new CreateNewsCommandHandler(_newsRepositoryMock.Object, _authorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNews_WhenAllConditionsAreMet()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var command = new CreateNewsCommand(
            "Test Title",
            "Test Content",
            authorId);

        var author = Author.Create(FirstName.Create("John").Value, LastName.Create("Doe").Value, "johndoe", Email.Create("johndoe@example.com").Value, "hashedPassword");
        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId))
            .ReturnsAsync(author);

        _newsRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.News>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _newsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.News>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpdateContentIsInvalid()
    {
        // Arrange
        var command = new CreateNewsCommand(
            "Test Title",
            "", // Invalid content
            Guid.NewGuid()
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Content.Empty", result.Error.Code);
        _newsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.News>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAuthorNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var command = new CreateNewsCommand(
            "Test Title",
            "Test Content",
            authorId
        );

        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId))
            .ReturnsAsync((Author)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author.NotFound", result.Error.Code);
        _newsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.News>()), Times.Never);
    }
}