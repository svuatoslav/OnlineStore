using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Purchase
{
    public int Id { get; set; }

    public int? Usersid { get; set; }

    public DateOnly Orderdate { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Purchaseproduct> Purchaseproducts { get; set; } = new List<Purchaseproduct>();

    public virtual User? Users { get; set; }
    public Purchase() { }
    public Purchase(int id, DateOnly orderdate, decimal price)
    {
        Id = id;
        Orderdate = orderdate;
        Price = price;
    }
}
