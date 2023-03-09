namespace NoteWebApp.Request {
    public class TaskItemRequest {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public byte Progress { get; set; }

        public DateTime? Deadline { get; set; }

        public byte Priority { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid TaskId { get; set; }

        public bool? IsDelete { get; set; }

        public bool validate() {
            if (Progress != DefaultData.PROGRESS_PROGRESS &&
                Progress != DefaultData.DONE_PROGRESS &&
                Progress != DefaultData.PLAN_PROGRESS &&
                Progress != DefaultData.REVIEW_PROGRESS) return false;
            if (Priority != DefaultData.LOW_PRIORITY &&
                Priority != DefaultData.HIGH_PRIORITY &&
                Priority != DefaultData.MEDIUM_PRIORITY) return false;
            if (DateTime.Compare(StartDate, CreatedAt) < 0) return false;
            return true;
        }
    }
}
