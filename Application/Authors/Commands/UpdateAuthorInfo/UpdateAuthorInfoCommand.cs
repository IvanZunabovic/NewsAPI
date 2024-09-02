using Application.Abstractions.Messaging;

namespace Application.Authors.Commands.UpdateAuthorInfo;

public sealed record UpdateAuthorInfoCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName) : ICommand;
