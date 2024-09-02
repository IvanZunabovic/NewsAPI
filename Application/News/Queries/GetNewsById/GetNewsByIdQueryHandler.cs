using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;
using Domain.Repositories;
using Domain.Shared;

namespace Application.News.Queries.GetNewsById;

public sealed class GetNewsByIdQueryHandler : IQueryHandler<GetNewsByIdQuery, NewsResponse>
{
    private readonly INewsRepository _newsRepository;

    public GetNewsByIdQueryHandler(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<Result<NewsResponse>> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetNewsByIdAsync(request.Id);

        if (news is null)
        {
            return Result.Failure<NewsResponse>(new Error(
                "News.NorFound",
                $"News with Id {request.Id} was not found"));
        }

        var response = new NewsResponse(
            news.Id,
            news.Title,
            news.Content.Value,
            news.CreatedAt);

        return response;
    }
}
