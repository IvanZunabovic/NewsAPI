using Application.Abstractions.Messaging;

namespace Application.News.Commands.CreateNews;

public sealed record CreateNewsCommand(
    string Title,
    string Content,
    DateTime CreatedAt,
    Guid AuthorId) : ICommand;
