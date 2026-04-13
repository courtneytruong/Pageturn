using Microsoft.EntityFrameworkCore;
using Pageturn.Core.Entities;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _dbContext;

    public BookRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book> GetByIdAsync(Guid id)
    {
        return await _dbContext.Books.FindAsync(id);
    }

    public async Task<Book> GetByExternalIdAsync(string externalId)
    {
        return await _dbContext.Books.FirstOrDefaultAsync(b => b.ExternalId == externalId);
    }

    public async Task CreateAsync(Book book)
    {
        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();
    }
}
