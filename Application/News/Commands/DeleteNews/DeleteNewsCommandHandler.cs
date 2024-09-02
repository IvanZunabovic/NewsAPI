using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;

namespace Application.News.Commands.DeleteNews;

public sealed class DeleteNewsCommandHandler : ICommandHandler<DeleteNewsCommand>
{
    private readonly INewsRepository _newsRepository;

    public DeleteNewsCommandHandler(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<Result> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetNewsByIdAsync(request.Id);

        if (news is null)
        {
            return Result.Failure(new Error(
                "News.NotFound",
                $"News with id {request.Id} does not exist."));
        }

        await _newsRepository.Delete(news);
        
        return Result.Success();
    }
}
