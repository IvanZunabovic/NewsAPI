using Application.Auth.Queries.Login;
using Application.Services;
using Domain.Repositories;
using Moq;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Auth;
public class LoginQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IHashingService> _hashingServiceMock;
    private readonly Mock<IClaimsService> _claimsServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _hashingServiceMock = new Mock<IHashingService>();
        _claimsServiceMock = new Mock<IClaimsService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new LoginQueryHandler(
            _hashingServiceMock.Object,
            _authorRepositoryMock.Object,
            _claimsServiceMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthorResponse_WhenLoginIsSuccessful()
    {
        // Arrange
        var query = new LoginQuery(
            "johndoe",
            "password123");

        var author = new Domain.Entities.Author(
            Guid.NewGuid(),
            Domain.ValueObjects.FirstName.Create("John").Value,
            Domain.ValueObjects.LastName.Create("Doe").Value,
            "johndoe",
            Domain.ValueObjects.Email.Create("john.doe@example.com").Value,
            "hashedpassword");

        _authorRepositoryMock.Setup(repo => repo.GetAuthorByUserNameAsync(query.UserName))
            .ReturnsAsync(author);
        _hashingServiceMock.Setup(service => service.Compare(author.PasswordHash, query.Password))
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.Value;
        Assert.Equal(author.Id, response.Id);
        Assert.Equal(author.FirstName.Value, response.FirstName);
        Assert.Equal(author.LastName.Value, response.LastName);
        Assert.Equal(author.Email.Value, response.Email);
        Assert.Equal(author.UserName, response.UserName);
        _claimsServiceMock.Verify(service => service.CreateClaims(It.IsAny<IHttpContextAccessor>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAuthorNotFound()
    {
        // Arrange
        var query = new LoginQuery(
            "johndoe",
            "password123");

        _authorRepositoryMock.Setup(repo => repo.GetAuthorByUserNameAsync(query.UserName))
            .ReturnsAsync((Domain.Entities.Author)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Author with user name johndoe not found.", result.Error?.Message);
        _claimsServiceMock.Verify(service => service.CreateClaims(It.IsAny<IHttpContextAccessor>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var query = new LoginQuery(
            "johndoe",
            "wrongpassword");

        var author = new Domain.Entities.Author(
            Guid.NewGuid(),
            Domain.ValueObjects.FirstName.Create("John").Value,
            Domain.ValueObjects.LastName.Create("Doe").Value,
            "johndoe",
            Domain.ValueObjects.Email.Create("john.doe@example.com").Value,
            "hashedpassword");

        _authorRepositoryMock.Setup(repo => repo.GetAuthorByUserNameAsync(query.UserName))
            .ReturnsAsync(author);
        _hashingServiceMock.Setup(service => service.Compare(author.PasswordHash, query.Password))
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The password is incorrect.", result.Error?.Message);
        _claimsServiceMock.Verify(service => service.CreateClaims(It.IsAny<IHttpContextAccessor>(), It.IsAny<string>()), Times.Never);
    }
}
