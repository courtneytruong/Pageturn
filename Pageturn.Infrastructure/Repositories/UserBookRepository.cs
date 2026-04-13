using Microsoft.EntityFrameworkCore;
using Pageturn.Core.Entities;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Repositories;

public class UserBookRepository : IUserBookRepository
{
    private readonly AppDbContext _dbContext;

    public UserBookRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserBook>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserBooks.Where(ub => ub.UserId == userId).ToListAsync();
    }

    public async Task<UserBook> GetByUserIdAndBookIdAsync(Guid userId, Guid bookId)
    {
        return await _dbContext.UserBooks.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId);
    }

    public async Task CreateAsync(UserBook userBook)
    {
        await _dbContext.UserBooks.AddAsync(userBook);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserBook userBook)
    {
        _dbContext.UserBooks.Update(userBook);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserBook userBook)
    {
        _dbContext.UserBooks.Remove(userBook);
        await _dbContext.SaveChangesAsync();
    }
}
