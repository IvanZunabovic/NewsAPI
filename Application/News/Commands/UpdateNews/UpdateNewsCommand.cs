using Application.Abstractions.Messaging;

namespace Application.News.Commands.UpdateNews;

public sealed record UpdateNewsCommand(
    Guid Id,
    string Title,
    string Content,
    Guid currentAuthorId) : ICommand;
