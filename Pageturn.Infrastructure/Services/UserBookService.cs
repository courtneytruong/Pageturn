using Pageturn.Core.DTOs.Books;
using Pageturn.Core.DTOs.UserBooks;
using Pageturn.Core.Entities;
using Pageturn.Core.Exceptions;
using Pageturn.Core.Interfaces;

namespace Pageturn.Infrastructure.Services;

public class UserBookService : IUserBookService
{
    private readonly IUserBookRepository _userBookRepository;
    private readonly IBookRepository _bookRepository;

    public UserBookService(IUserBookRepository userBookRepository, IBookRepository bookRepository)
    {
        _userBookRepository = userBookRepository;
        _bookRepository = bookRepository;
    }

    public async Task<List<UserBookResponseDto>> GetLibraryAsync(Guid userId)
    {
        var userBooks = await _userBookRepository.GetByUserIdAsync(userId);

        var result = new List<UserBookResponseDto>();
        foreach (var userBook in userBooks)
        {
            var book = await _bookRepository.GetByIdAsync(userBook.BookId);
            if (book != null)
            {
                result.Add(new UserBookResponseDto
                {
                    Id = userBook.Id,
                    Book = new BookResponseDto
                    {
                        Id = book.Id,
                        ExternalId = book.ExternalId,
                        Title = book.Title,
                        Author = book.Author,
                        CoverImageUrl = book.CoverImageUrl
                    },
                    Status = userBook.Status,
                    CreatedAt = userBook.CreatedAt
                });
            }
        }

        return result;
    }

    public async Task<UserBookResponseDto> AddBookAsync(Guid userId, AddUserBookRequestDto request)
    {
        var book = await _bookRepository.GetByExternalIdAsync(request.ExternalId);

        if (book == null)
        {
            book = new Book
            {
                ExternalId = request.ExternalId,
                Title = request.Title,
                Author = request.Author,
                CoverImageUrl = request.CoverImageUrl
            };
            await _bookRepository.CreateAsync(book);
        }

        var existingUserBook = await _userBookRepository.GetByUserIdAndBookIdAsync(userId, book.Id);
        if (existingUserBook != null)
            throw new ConflictException($"User already has this book");

        var userBook = new UserBook
        {
            UserId = userId,
            BookId = book.Id,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userBookRepository.CreateAsync(userBook);

        return new UserBookResponseDto
        {
            Id = userBook.Id,
            Book = new BookResponseDto
            {
                Id = book.Id,
                ExternalId = book.ExternalId,
                Title = book.Title,
                Author = book.Author,
                CoverImageUrl = book.CoverImageUrl
            },
            Status = userBook.Status,
            CreatedAt = userBook.CreatedAt
        };
    }

    public async Task<UserBookResponseDto> UpdateStatusAsync(Guid userId, Guid bookId, UpdateUserBookStatusDto request)
    {
        var userBook = await _userBookRepository.GetByUserIdAndBookIdAsync(userId, bookId);

        if (userBook == null)
            throw new NotFoundException($"UserBook not found");

        if (userBook.UserId != userId)
            throw new UnauthorizedException("You do not have permission to update this book");

        userBook.Status = request.Status;
        userBook.UpdatedAt = DateTime.UtcNow;

        await _userBookRepository.UpdateAsync(userBook);

        var book = await _bookRepository.GetByIdAsync(bookId);

        return new UserBookResponseDto
        {
            Id = userBook.Id,
            Book = new BookResponseDto
            {
                Id = book.Id,
                ExternalId = book.ExternalId,
                Title = book.Title,
                Author = book.Author,
                CoverImageUrl = book.CoverImageUrl
            },
            Status = userBook.Status,
            CreatedAt = userBook.CreatedAt
        };
    }

    public async Task RemoveBookAsync(Guid userId, Guid bookId)
    {
        var userBook = await _userBookRepository.GetByUserIdAndBookIdAsync(userId, bookId);

        if (userBook == null)
            throw new NotFoundException($"UserBook not found");

        if (userBook.UserId != userId)
            throw new UnauthorizedException("You do not have permission to delete this book");

        await _userBookRepository.DeleteAsync(userBook);
    }
}
