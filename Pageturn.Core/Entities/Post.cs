namespace Pageturn.Core.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public string Content { get; set; }
    public bool IsFinished { get; set; }
    public DateTime ReadOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
