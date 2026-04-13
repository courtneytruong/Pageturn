using Microsoft.EntityFrameworkCore;
using Pageturn.Core.Entities;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _dbContext;

    public PostRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Posts.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByUserIdAndBookIdAsync(Guid userId, Guid bookId)
    {
        return await _dbContext.Posts.Where(p => p.UserId == userId && p.BookId == bookId).ToListAsync();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await _dbContext.Posts.FindAsync(id);
    }

    public async Task CreateAsync(Post post)
    {
        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Post post)
    {
        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }
}
