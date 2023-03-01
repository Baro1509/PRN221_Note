using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Task
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();

    public virtual User User { get; set; } = null!;
}
