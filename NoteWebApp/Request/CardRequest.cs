namespace NoteWebApp.Request
{
    public class CardRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? Color { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid NoteId { get; set; }
    }
}
