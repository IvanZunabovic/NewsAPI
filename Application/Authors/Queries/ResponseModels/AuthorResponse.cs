namespace Application.Authors.Queries.ResponseModels;

public sealed record AuthorResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName);