using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? BirthPlace { get; set; }

    public int? BirthYear { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
