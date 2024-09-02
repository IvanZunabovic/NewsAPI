using Application.Abstractions.Messaging;

namespace Application.News.Commands.DeleteNews;

public sealed record DeleteNewsCommand(Guid Id) : ICommand;
