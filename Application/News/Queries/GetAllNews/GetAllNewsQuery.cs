using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;

namespace Application.News.Queries.GetAllNews;

public sealed record GetAllNewsQuery(int pageNumber, int pageSize) : IQuery<IEnumerable<NewsResponse>>;
