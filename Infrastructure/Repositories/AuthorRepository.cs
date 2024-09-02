using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IQueryable<Author> _authors;

    public AuthorRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _authors = _dbContext.Set<Author>();
    }

    public async Task AddAsync(Author author)
    {
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize)
    {
        return await _authors
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(Guid id)
    {
        var query = from author in _authors
                   where author.Id == id
                   select author;

        return await query.SingleOrDefaultAsync();
    }

    public async Task<Author?> GetAuthorByUserNameAsync(string userName)
    {
        var query = from author in _dbContext.Authors
                    where author.UserName == userName
                    select author;

        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> HasUserWithExistingEmailOrUserName(Email email, string userName)
    {
        var query = from author in _dbContext.Authors
                    where author.UserName == userName || author.Email.Value == email.Value
                    select author;

        return await query.AnyAsync();
    }

    public async Task UpdateAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
