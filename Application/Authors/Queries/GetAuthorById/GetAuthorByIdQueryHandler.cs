using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Authors.Queries.GetAuthorById;

public sealed class GetAuthorByIdQueryHandler : IQueryHandler<GetAuthorByIdQuery, AuthorResponse>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<AuthorResponse>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(request.AuthorId);

        if (author is null)
        {
            return Result.Failure<AuthorResponse>(new Error(
                "Author.NotFound",
                $"Author with Id {request.AuthorId} was not found"));
        }

        var response = new AuthorResponse(
            author.Id,
            author.FirstName.Value,
            author.LastName.Value,
            author.Email.Value,
            author.UserName);

        return response;
    }
}
