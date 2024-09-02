using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IAuthorRepository
{
    Task AddAsync(Author author);
    Task UpdateAsync(CancellationToken cancellationToken);
    Task<Author?> GetByIdAsync(Guid id);
    Task<IEnumerable<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize);
    Task<bool> HasUserWithExistingEmailOrUserName(Email value, string userName);
    Task<Author?> GetAuthorByUserNameAsync(string userName);
}
