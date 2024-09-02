using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;
using Domain.Repositories;
using Domain.Shared;

namespace Application.News.Queries.GetAllNews;

public sealed class GetAllNewsQueryHandler : IQueryHandler<GetAllNewsQuery, IEnumerable<NewsResponse>>
{
    private readonly INewsRepository _newsRepository;

    public GetAllNewsQueryHandler(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<Result<IEnumerable<NewsResponse>>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetAllNewsAsync(request.pageNumber, request.pageSize);

        var response = news.Select(news => new NewsResponse(
            news.Id,
            news.Title,
            news.Content.Value,
            news.CreatedAt))
            .ToList();

        return response;
    }
}
