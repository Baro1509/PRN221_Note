namespace NoteWebApp.Response {
    public class TaskItemResponse {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? Deadline { get; set; }

        public byte Priority { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid TaskId { get; set; }
    }
}
