using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string Suppliername { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
