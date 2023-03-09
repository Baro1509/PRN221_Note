namespace Repository.Models;

public partial class TaskItem
{
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

    public virtual Task Task { get; set; } = null!;
}
