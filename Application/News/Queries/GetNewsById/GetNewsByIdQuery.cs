using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;

namespace Application.News.Queries.GetNewsById;

public sealed record GetNewsByIdQuery(Guid Id) : IQuery<NewsResponse>;
