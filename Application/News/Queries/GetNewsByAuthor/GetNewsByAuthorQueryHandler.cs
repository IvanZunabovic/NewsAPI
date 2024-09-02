using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;
using Domain.Repositories;
using Domain.Shared;

namespace Application.News.Queries.GetNewsByAuthor;

public sealed class GetNewsByAuthorQueryHandler : IQueryHandler<GetNewsByAuthorQuery, IEnumerable<NewsResponse>>
{
    private readonly INewsRepository _newsRepository;

    public GetNewsByAuthorQueryHandler(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<Result<IEnumerable<NewsResponse>>> Handle(GetNewsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetNewsByAuthorAsync(request.AuthorId, request.pageNumber, request.pageSize);

        var response = news.Select(news => new NewsResponse(
            news.Id,
            news.Title,
            news.Content.Value,
            news.CreatedAt))
            .ToList();

        return response;
    }
}
