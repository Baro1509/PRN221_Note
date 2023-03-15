namespace NoteWebApp.Response
{
    public class CardResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }
        public string? RawContent { get; set; }

        public string? Color { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid NoteId { get; set; }
        public String NoteTitle { get; set; }
        public bool? IsDelete { get; set; }
    }
}
