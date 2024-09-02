using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class NewsRepository : INewsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IQueryable<News> _news;

    public NewsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _news = _dbContext.Set<News>();
    }

    public async Task AddAsync(News news)
    {
        await _dbContext.Set<News>().AddAsync(news);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(News news)
    {
        _dbContext.News.Remove(news);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<News>> GetAllNewsAsync(int pageNumber, int pageSize)
    {
        return await _news
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetNewsByAuthorAsync(Guid authorId, int pageNumber, int pageSize)
    {
        return await _news
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<News?> GetNewsByIdAsync(Guid id)
    {
        var query = from news in _news
                    where news.Id == id
                    select news;

        return await query.SingleOrDefaultAsync();
    }

    public async Task UpdateAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
