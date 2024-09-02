using Application.News.Commands.DeleteNews;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Moq;

namespace Application.UnitTest.News;

public class DeleteNewsCommandHandlerTest
{
    private readonly Mock<INewsRepository> _newsRepositoryMock;

    public DeleteNewsCommandHandlerTest()
    {
        _newsRepositoryMock = new();
    }
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenNewsDoesntExist()
    {
        var command = new DeleteNewsCommand(Guid.NewGuid());

        var handler = new DeleteNewsCommandHandler(_newsRepositoryMock.Object);

        Result result = await handler.Handle(command, default);

        Assert.Equal("News.NotFound", result.Error);
    }        
}
