using Pageturn.Core.Entities;

namespace Pageturn.Core.Interfaces;

public interface IUserBookRepository
{
    Task<IEnumerable<UserBook>> GetByUserIdAsync(Guid userId);
    Task<UserBook> GetByUserIdAndBookIdAsync(Guid userId, Guid bookId);
    Task CreateAsync(UserBook userBook);
    Task UpdateAsync(UserBook userBook);
    Task DeleteAsync(UserBook userBook);
}
