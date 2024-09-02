using Domain.Entities;

namespace Domain.Repositories;

public interface INewsRepository
{
    Task AddAsync(News news);
    Task UpdateAsync(CancellationToken cancellationToken);
    Task Delete(News id);
    Task<News?> GetNewsByIdAsync(Guid id);
    Task<IEnumerable<News>> GetNewsByAuthorAsync(Guid authorId, int pageNumber, int pageSize);
    Task<IEnumerable<News>> GetAllNewsAsync(int pageNumber, int pageSize);
}
