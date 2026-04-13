using Pageturn.Core.Entities;

namespace Pageturn.Core.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Post>> GetByUserIdAndBookIdAsync(Guid userId, Guid bookId);
    Task<Post> GetByIdAsync(Guid id);
    Task CreateAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}
