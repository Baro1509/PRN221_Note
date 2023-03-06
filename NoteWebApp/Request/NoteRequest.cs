namespace NoteWebApp.Request
{
    public class NoteRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
    }
}
