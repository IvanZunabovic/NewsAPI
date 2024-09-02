namespace Application.News.Queries.ResponseModels;

public sealed record NewsResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt);