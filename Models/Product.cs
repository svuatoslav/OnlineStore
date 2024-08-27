using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public int? Categoryid { get; set; }

    public string Productname { get; set; } = null!;

    public string Company { get; set; } = null!;

    public int Productcount { get; set; }

    public decimal Price { get; set; }

    public bool Isdiscounted { get; set; }

    public virtual Productcategory? Category { get; set; }

    public virtual ICollection<Purchaseproduct> Purchaseproducts { get; set; } = new List<Purchaseproduct>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
    public Product() { }
    public Product(string productname, int productcount, decimal price)
    {
        Productname = productname;
        Productcount = productcount;
        Price = price;
    }
}
