using Repository.Models;

namespace NoteWebApp.Response {
    public class TaskWithTaskItemResponse {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public byte Progress { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime UpdatedAt { get; set; }

        public byte Priority { get; set; }

        public Guid UserId { get; set; }

        public bool? IsDelete { get; set; }

        public virtual ICollection<TaskItemResponse> TaskItems { get; } = new List<TaskItemResponse>();
    }
}
