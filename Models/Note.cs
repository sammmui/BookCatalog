using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class Note
{
    public int Id { get; set; }

    public int UserBookId { get; set; }

    public string Content { get; set; } = null!;

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserBook UserBook { get; set; } = null!;
}
