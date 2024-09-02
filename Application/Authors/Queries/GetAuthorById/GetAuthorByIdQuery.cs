using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;

namespace Application.Authors.Queries.GetAuthorById;

public sealed record GetAuthorByIdQuery(Guid AuthorId) : IQuery<AuthorResponse>;
