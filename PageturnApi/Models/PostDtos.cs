namespace PageturnApi.Models;

public class CreatePostDto
{
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
}

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
}
