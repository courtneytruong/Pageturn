using Pageturn.Core.Entities;

namespace Pageturn.Core.Interfaces;

public interface IBookRepository
{
    Task<Book> GetByIdAsync(Guid id);
    Task<Book> GetByExternalIdAsync(string externalId);
    Task CreateAsync(Book book);
}
