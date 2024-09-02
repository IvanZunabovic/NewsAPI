using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.News.Commands.UpdateNews;

public sealed class UpdateNewsCommandHandler : ICommandHandler<UpdateNewsCommand>
{
    private readonly INewsRepository _newsRepository;

    public UpdateNewsCommandHandler(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<Result> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
    {
        var content = Content.Create(request.Content);

        if(!content.IsSuccess)
        {
            return Result.Failure(content.Error);
        }

        var news = await _newsRepository.GetNewsByIdAsync(request.Id);

        if (news is null)
        {
            return Result.Failure(new Error(
                "News.NotFound",
                $"News with id {request.Id} does not exist."));
        }

        if (news.AuthorId != request.currentAuthorId)
        {
            return Result.Failure(new Error(
                "News.CantEdit",
                $"Not able to edit news."));
        }

        news.Title = request.Title;
        news.Content = content.Value;

        await _newsRepository.UpdateAsync(cancellationToken);
        return Result.Success();
    }
}
