using Application.Abstractions.Messaging;
using Application.News.Queries.ResponseModels;

namespace Application.News.Queries.GetNewsByAuthor;

public sealed record GetNewsByAuthorQuery(Guid AuthorId, int pageNumber, int pageSize) : IQuery<IEnumerable<NewsResponse>>;
