using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Note
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<Card> Cards { get; } = new List<Card>();

    public virtual User User { get; set; } = null!;
}
