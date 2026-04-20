using Pageturn.Core.DTOs.Books;
using Pageturn.Core.Entities;

namespace Pageturn.Core.DTOs.UserBooks;

public class UserBookResponseDto
{
    public Guid Id { get; set; }
    public BookResponseDto Book { get; set; }
    public ReadingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
