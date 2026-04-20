using Pageturn.Core.DTOs.Books;
using Pageturn.Core.Exceptions;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly GoogleBooksService _googleBooksService;
    private readonly IBookRepository _bookRepository;

    public BookService(GoogleBooksService googleBooksService, IBookRepository bookRepository)
    {
        _googleBooksService = googleBooksService;
        _bookRepository = bookRepository;
    }

    public async Task<List<BookSearchResultDto>> SearchBooksAsync(string query)
    {
        return await _googleBooksService.SearchBooksAsync(query);
    }

    public async Task<BookResponseDto> GetBookAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
            throw new NotFoundException($"Book with id {id} not found");

        return new BookResponseDto
        {
            Id = book.Id,
            ExternalId = book.ExternalId,
            Title = book.Title,
            Author = book.Author,
            CoverImageUrl = book.CoverImageUrl
        };
    }
}
