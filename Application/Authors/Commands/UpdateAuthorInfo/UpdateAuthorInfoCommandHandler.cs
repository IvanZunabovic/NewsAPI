using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Authors.Commands.UpdateAuthorInfo;

public sealed class UpdateAuthorInfoCommandHandler : ICommandHandler<UpdateAuthorInfoCommand>
{
    private readonly IAuthorRepository _authorRepository;

    public UpdateAuthorInfoCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result> Handle(UpdateAuthorInfoCommand request, CancellationToken cancellationToken)
    {
        var firstName = FirstName.Create(request.FirstName);
        var lastName = LastName.Create(request.LastName);
        var email = Email.Create(request.Email);

        if (!firstName.IsSuccess || !lastName.IsSuccess || !email.IsSuccess)
        {
            return Result.Failure(firstName.Error != Error.None 
                ? firstName.Error : lastName.Error != Error.None 
                    ? lastName.Error : email.Error);
        }

        var author = await _authorRepository.GetByIdAsync(request.Id);

        if (author is null) 
        {
            return Result.Failure(new Error(
                "Author.NotFound",
                $"Author with Id {request.Id} does not exist."));
        }


        author.FirstName = firstName.Value;
        author.LastName = lastName.Value;
        author.Email = email.Value;
        author.UserName = request.UserName;


        await _authorRepository.UpdateAsync(cancellationToken);
        return Result.Success();
    }
}
