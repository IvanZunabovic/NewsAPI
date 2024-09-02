using Application.Abstractions.Messaging;
using Application.Authors.Queries.ResponseModels;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Authors.Queries.GetAllAuthors;

public sealed class GetAllAuthorsQueryHandler : IQueryHandler<GetAllAuthorsQuery, IEnumerable<AuthorResponse>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAllAuthorsQueryHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<IEnumerable<AuthorResponse>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _authorRepository.GetAllAuthorsAsync(request.pageNumber, request.pageSize);

        var response = authors.Select(author => new AuthorResponse(
            author.Id,
            author.FirstName.Value,
            author.LastName.Value,
            author.Email.Value,
            author.UserName))
            .ToList();

        return response;
    }
}
