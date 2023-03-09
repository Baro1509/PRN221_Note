namespace NoteWebApp.Response
{
    public class NoteCardResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public ICollection<CardResponse> Cards { get; set; }
    }
}
