using Pageturn.Core.DTOs.UserBooks;

namespace Pageturn.Core.Interfaces;

public interface IUserBookService
{
    Task<List<UserBookResponseDto>> GetLibraryAsync(Guid userId);
    Task<UserBookResponseDto> AddBookAsync(Guid userId, AddUserBookRequestDto request);
    Task<UserBookResponseDto> UpdateStatusAsync(Guid userId, Guid bookId, UpdateUserBookStatusDto request);
    Task RemoveBookAsync(Guid userId, Guid bookId);
}
