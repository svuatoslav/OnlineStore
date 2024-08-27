using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Supply
{
    public int Id { get; set; }

    public int? Supplierid { get; set; }

    public int? Productid { get; set; }

    public decimal Price { get; set; }

    public DateOnly Supplydate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
