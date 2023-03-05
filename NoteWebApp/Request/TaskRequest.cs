namespace NoteWebApp.Request {
    public class TaskRequest {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public byte Priority { get; set; }

        public Guid UserId { get; set; }
    }
}
