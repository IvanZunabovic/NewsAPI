using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.News.Commands.CreateNews;

public sealed class CreateNewsCommandHandler : ICommandHandler<CreateNewsCommand>
{
    private readonly INewsRepository _newsRepository;
    private readonly IAuthorRepository _authorRepository;
    public CreateNewsCommandHandler(INewsRepository newsRepository, IAuthorRepository authorRepository)
    {
        _newsRepository = newsRepository;
        _authorRepository = authorRepository;
    }

    public async Task<Result> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
    {
        var content = Content.Create(request.Content);

        if (!content.IsSuccess)
        {
            return Result.Failure(content.Error);
        }

        var author = await _authorRepository.GetByIdAsync(request.AuthorId);

        if (author is null)
        {
            return Result.Failure(new Error(
                "Author.NotFound",
                $"Author with id {request.AuthorId} does not exist"));
        }

        var news = Domain.Entities.News.Create(request.Title, content.Value, request.CreatedAt, request.AuthorId);

        await _newsRepository.AddAsync(news);
        return Result.Success();
    }
}
