using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Purchaseproduct
{
    public int Purchaseid { get; set; }

    public int Productid { get; set; }

    public int Quantityproduct { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Purchase Purchase { get; set; } = null!;
    public Purchaseproduct() { }
    public Purchaseproduct(int quantityproduct, Product product, Purchase purchase)
    {
        Quantityproduct = quantityproduct;
        Product = product;
        Purchase = purchase;
    }
}
