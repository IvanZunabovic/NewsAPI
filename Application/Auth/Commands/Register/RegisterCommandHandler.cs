using Application.Abstractions.Messaging;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IHashingService _hashingService;

    public RegisterCommandHandler(IAuthorRepository authorRepository, IHashingService hashingService)
    {
        _authorRepository = authorRepository;
        _hashingService = hashingService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        if (!email.IsSuccess)
        {
            return Result.Failure(email.Error);
        }

        var HasUserWithExistingEmailOrUserName = await _authorRepository
            .HasUserWithExistingEmailOrUserName(email.Value, request.UserName);

        if (HasUserWithExistingEmailOrUserName)
        {
            return Result.Failure(new Error(
                "Author.ExistingUser",
                $"Author with same email or user name already exists."));
        }

        var firstName = FirstName.Create(request.FirstName ?? "");
        var lastName = LastName.Create(request.LastName ?? "");

        if (!firstName.IsSuccess || !lastName.IsSuccess)
        {
            return Result.Failure(!firstName.IsSuccess ? firstName.Error : lastName.Error);
        }

        var author = Author.Create(
            firstName.Value,
            lastName.Value,
            request.UserName,
            email.Value,
            _hashingService.HashPassword(request.Password));

        await _authorRepository.AddAsync(author);
        return Result.Success();
    }
}
