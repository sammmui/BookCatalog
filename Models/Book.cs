using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Year { get; set; }

    public int Pages { get; set; }

    public string? Isbn { get; set; }

    public string? Description { get; set; }

    public string? CoverPath { get; set; }

    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
