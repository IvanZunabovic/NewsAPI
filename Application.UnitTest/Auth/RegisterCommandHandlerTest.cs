using Application.Auth.Commands.Register;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTest.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IHashingService> _hashingServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _hashingServiceMock = new Mock<IHashingService>();
        _handler = new RegisterCommandHandler(_authorRepositoryMock.Object, _hashingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "john.doe@example.com",
            "password123",
            "John",
            "Doe");

        _authorRepositoryMock.Setup(repo => repo.HasUserWithExistingEmailOrUserName(It.IsAny<Email>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        _hashingServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _authorRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "", // Invalid email
            "password123",
            "John",
            "Doe");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email is empty.", result.Error?.Message); // Adjust based on actual error message
        _authorRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserWithExistingEmailOrUserNameExists()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "john.doe@example.com",
            "password123",
            "John",
            "Doe");

        _authorRepositoryMock.Setup(repo => repo.HasUserWithExistingEmailOrUserName(It.IsAny<Email>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author with same email or user name already exists.", result.Error?.Message); // Adjust based on actual error message
        _authorRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFirstNameIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "john.doe@example.com",
            "password123",
            "", // Invalid first name
            "Doe");

        _authorRepositoryMock.Setup(repo => repo.HasUserWithExistingEmailOrUserName(It.IsAny<Email>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        _hashingServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("FirstName is empty.", result.Error?.Message); // Adjust based on actual error message
        _authorRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLastNameIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "john.doe@example.com",
            "password123",
            "John",
            "" // Invalid last name
            );

        _authorRepositoryMock.Setup(repo => repo.HasUserWithExistingEmailOrUserName(It.IsAny<Email>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        _hashingServiceMock.Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedpassword");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("LastName is empty.", result.Error?.Message); // Adjust based on actual error message
        _authorRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Author>()), Times.Never);
    }
}
