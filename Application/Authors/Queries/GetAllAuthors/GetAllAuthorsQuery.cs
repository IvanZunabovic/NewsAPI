using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;

namespace Application.Authors.Queries.GetAllAuthors;

public sealed record GetAllAuthorsQuery(int pageNumber, int pageSize) : IQuery<IEnumerable<AuthorResponse>>;
