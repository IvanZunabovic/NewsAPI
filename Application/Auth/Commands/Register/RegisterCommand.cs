using Application.Abstractions.Messaging;

namespace Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string? FirstName,
    string? LastName) : ICommand;
