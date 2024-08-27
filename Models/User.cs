using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? Age { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public User() { }
    public User(string firstname, string lastname, string? email, ICollection<Purchase> purchases)
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        foreach (var purchase in purchases)
            Purchases.Add(purchase);
    }
}
