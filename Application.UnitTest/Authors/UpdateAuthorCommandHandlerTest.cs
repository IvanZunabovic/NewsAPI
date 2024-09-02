using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Authors.Commands.UpdateAuthorInfo;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using Moq;
using Xunit;

namespace Application.UnitTest.Authors;
public class UpdateAuthorInfoCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly UpdateAuthorInfoCommandHandler _handler;

    public UpdateAuthorInfoCommandHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _handler = new UpdateAuthorInfoCommandHandler(_authorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAuthorInfo_WhenAllInputsAreValid()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var author = Author.Create(FirstName.Create("John").Value, LastName.Create("Doe").Value, "john_doe", Email.Create("john.doe@example.com").Value, "hashedpassword");

        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);

        var command = new UpdateAuthorInfoCommand(
            authorId,
            "Jane",
            "Smith",
            "jane.smith@example.com",
            "jane_smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", author.FirstName.Value);
        Assert.Equal("Smith", author.LastName.Value);
        Assert.Equal("jane.smith@example.com", author.Email.Value);
        Assert.Equal("jane_smith", author.UserName);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFirstNameValidationFails()
    {
        // Arrange
        var command = new UpdateAuthorInfoCommand(
            Guid.NewGuid(),
            "",
            "Smith",
            "jane.smith@example.com",
            "jane_smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.FirstName.Empty.Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLastNameValidationFails()
    {
        // Arrange
        var command = new UpdateAuthorInfoCommand(
            Guid.NewGuid(),
            "Jane",
            "",
            "jane.smith@example.com",
            "jane_smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.LastName.Empty.Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailValidationFails()
    {
        // Arrange
        var command = new UpdateAuthorInfoCommand(
            Guid.NewGuid(),
            "Jane",
            "Smith",
            "invalid-email",
            "jane_smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Email.InvalidFormat.Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAuthorNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null);

        var command = new UpdateAuthorInfoCommand(
            authorId,
            "Jane",
            "Smith",
            "jane.smith@example.com",
            "jane_smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author.NotFound", result.Error.Code);
    }
}
