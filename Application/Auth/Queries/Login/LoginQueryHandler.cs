using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;
using Application.Services;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Auth.Queries.Login;

public sealed class LoginQueryHandler : IQueryHandler<LoginQuery, AuthorResponse>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IHashingService _hashingService;
    private readonly IClaimsService _claimsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginQueryHandler(IHashingService hashingService, IAuthorRepository authorRepository, IClaimsService claimsService, IHttpContextAccessor httpContextAccessor)
    {
        _hashingService = hashingService;
        _authorRepository = authorRepository;
        _claimsService = claimsService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AuthorResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetAuthorByUserNameAsync(request.UserName);

        if (author is null)
        {
            return Result.Failure<AuthorResponse>(new Error(
                "Author.NotFound",
                $"Author with user name {request.UserName} not found."));
        }

        if (_hashingService.Compare(author.PasswordHash, request.Password))
        {
            await _claimsService.CreateClaims(_httpContextAccessor, $"{author.FirstName.Value} {author.LastName.Value}");

            return new AuthorResponse(
                author.Id,
                author.FirstName.Value,
                author.LastName.Value,
                author.Email.Value,
                author.UserName);
        }

        return Result.Failure<AuthorResponse>(new Error(
            "Author.PasswordIncorrect",
            "The password is incorrect."));
    }
}
