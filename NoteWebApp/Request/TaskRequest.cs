namespace NoteWebApp.Request {
    public class TaskRequest {
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

        public bool validation() {
            if (Progress != DefaultData.PROGRESS_PROGRESS || 
                Progress != DefaultData.DONE_PROGRESS || 
                Progress != DefaultData.PLAN_PROGRESS || 
                Progress != DefaultData.REVIEW_PROGRESS) return false;
            if (Priority != DefaultData.LOW_PRIORITY ||
                Priority != DefaultData.HIGH_PRIORITY ||
                Priority != DefaultData.MEDIUM_PRIORITY) return false;
            return true;
        }
    }
}
