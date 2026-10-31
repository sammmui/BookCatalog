using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class ReadingLog
{
    public int Id { get; set; }

    public int UserBookId { get; set; }

    public DateOnly LogDate { get; set; }

    public int PagesRead { get; set; }

    public virtual UserBook UserBook { get; set; } = null!;
}
