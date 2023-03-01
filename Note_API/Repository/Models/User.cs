using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Note> Notes { get; } = new List<Note>();

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();
}
