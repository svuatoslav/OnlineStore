using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;

namespace OnlineStore;

public partial class OnlineStoreContext : DbContext
{
    private string _connectionString;
    public OnlineStoreContext(string Connect) { _connectionString = Connect; }
    public OnlineStoreContext() { }

    public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options) : base(options) { }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productcategory> Productcategories { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Purchaseproduct> Purchaseproducts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Company)
                .HasMaxLength(20)
                .HasDefaultValueSql("'unknown'::character varying")
                .HasColumnName("company");
            entity.Property(e => e.Isdiscounted)
                .HasDefaultValue(false)
                .HasColumnName("isdiscounted");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.Productcount)
                .HasDefaultValue(0)
                .HasColumnName("productcount");
            entity.Property(e => e.Productname)
                .HasMaxLength(30)
                .HasColumnName("productname");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("products_categoryid_fkey");
        });

        modelBuilder.Entity<Productcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productcategories_pkey");

            entity.ToTable("productcategories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(30)
                .HasColumnName("categoryname");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchases_pkey");

            entity.ToTable("purchases");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Orderdate).HasColumnName("orderdate");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.Usersid).HasColumnName("usersid");

            entity.HasOne(d => d.Users).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.Usersid)
                .HasConstraintName("purchases_usersid_fkey");
        });

        modelBuilder.Entity<Purchaseproduct>(entity =>
        {
            entity.HasKey(e => new { e.Purchaseid, e.Productid }).HasName("purchaseproduct_pkey");

            entity.ToTable("purchaseproduct");

            entity.Property(e => e.Purchaseid).HasColumnName("purchaseid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantityproduct)
                .HasDefaultValue(0)
                .HasColumnName("quantityproduct");

            entity.HasOne(d => d.Product).WithMany(p => p.Purchaseproducts)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchaseproduct_productid_fkey");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Purchaseproducts)
                .HasForeignKey(d => d.Purchaseid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchaseproduct_purchaseid_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suppliers_pkey");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.Inn, "suppliers_inn_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(30)
                .HasColumnName("address");
            entity.Property(e => e.Contact)
                .HasMaxLength(30)
                .HasColumnName("contact");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Suppliername)
                .HasMaxLength(30)
                .HasColumnName("suppliername");
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplies_pkey");

            entity.ToTable("supplies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Supplierid).HasColumnName("supplierid");
            entity.Property(e => e.Supplydate).HasColumnName("supplydate");

            entity.HasOne(d => d.Product).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("supplies_productid_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.Supplierid)
                .HasConstraintName("supplies_supplierid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(30)
                .HasColumnName("address");
            entity.Property(e => e.Age)
                .HasDefaultValue(18)
                .HasColumnName("age");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(30)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(30)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Userpassword)
                .HasMaxLength(30)
                .HasDefaultValueSql("'unknown'::character varying")
                .HasColumnName("userpassword");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
