using Application.Auth.Commands.Register;
using Application.Auth.Commands.Signout;
using Application.Auth.Queries.Login;
using Application.Authors.Queries.ResponseModels;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Presentation.Models;

namespace Presentation.UnitTest;
public class AuthControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new AuthController(_senderMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenCommandIsSuccessful()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "password123",
            "John",
            "Doe",
            "john.doe@example.com");

        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Register(command, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.NotNull(okResult);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenCommandFails()
    {
        // Arrange
        var command = new RegisterCommand(
            "johndoe",
            "password123",
            "John",
            "Doe",
            "john.doe@example.com");

        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("RegistrationFailed", "Registration failed")));

        // Act
        var result = await _controller.Register(command, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(new Error("RegistrationFailed", "Registration failed"), badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var model = new LoginModel(
            "johndoe",
            "password123");

        var authorResponse = new AuthorResponse(
            Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "johndoe");

        _senderMock.Setup(s => s.Send(It.Is<LoginQuery>(q => q.UserName == model.UserName && q.Password == model.Password), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(authorResponse));

        // Act
        var result = await _controller.Login(model, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(authorResponse, okResult.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
    {
        // Arrange
        var model = new LoginModel(
            "johndoe",
            "wrongpassword");

        _senderMock.Setup(s => s.Send(It.Is<LoginQuery>(q => q.UserName == model.UserName && q.Password == model.Password), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthorResponse>(new Error("LoginFailed", "Login failed")));

        // Act
        var result = await _controller.Login(model, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(new Error("LoginFailed", "Login failed"), badRequestResult.Value);
    }

    [Fact]
    public async Task Signout_ShouldReturnOk_WhenSignoutIsSuccessful()
    {
        // Arrange
        _senderMock.Setup(s => s.Send(It.IsAny<SignoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Signout(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.NotNull(okResult);
    }
}
