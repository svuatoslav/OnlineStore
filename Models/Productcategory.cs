using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Productcategory
{
    public int Id { get; set; }

    public string Categoryname { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
