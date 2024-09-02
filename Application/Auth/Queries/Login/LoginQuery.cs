using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;

namespace Application.Auth.Queries.Login;

public sealed record LoginQuery(string UserName, string Password) : IQuery<AuthorResponse>;
