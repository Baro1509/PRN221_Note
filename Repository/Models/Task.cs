namespace Repository.Models;

public partial class Task
{
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

    public virtual ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();

    public virtual User User { get; set; } = null!;
}
