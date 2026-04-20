using Pageturn.Core.DTOs.Books;

namespace Pageturn.Core.Interfaces;

public interface IBookService
{
    Task<List<BookSearchResultDto>> SearchBooksAsync(string query);
    Task<BookResponseDto> GetBookAsync(Guid id);
}
