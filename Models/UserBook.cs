using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class UserBook
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public string Status { get; set; } = null!;

    public int? Rating { get; set; }

    public int CurrentPage { get; set; }

    public bool IsFavourite { get; set; }

    public DateOnly? StartedAt { get; set; }

    public DateOnly? FinishedAt { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<ReadingLog> ReadingLogs { get; set; } = new List<ReadingLog>();

    public virtual User User { get; set; } = null!;
}
