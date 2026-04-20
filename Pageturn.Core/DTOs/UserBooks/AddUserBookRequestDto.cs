using Pageturn.Core.Entities;

namespace Pageturn.Core.DTOs.UserBooks;

public class AddUserBookRequestDto
{
    public string ExternalId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string CoverImageUrl { get; set; }
    public ReadingStatus Status { get; set; }
}
