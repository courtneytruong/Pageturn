using Pageturn.Core.Entities;

namespace Pageturn.Core.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
}
