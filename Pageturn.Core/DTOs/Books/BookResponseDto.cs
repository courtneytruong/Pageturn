namespace Pageturn.Core.DTOs.Books;

public class BookResponseDto
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string CoverImageUrl { get; set; }
}
