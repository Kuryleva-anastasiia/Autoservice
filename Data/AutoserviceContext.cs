using System;
using Microsoft.EntityFrameworkCore;
using Autoservice.Models;

namespace Autoservice.Data
{
    public class AutoserviceContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AutoserviceContext(DbContextOptions<AutoserviceContext> options)
            : base(options)
        {
        }

        public DbSet<Autoservice.Models.Clients> Clients { get; set; } = default!;
        public DbSet<Autoservice.Models.Employees> Employees { get; set; } = default!;
        public DbSet<Autoservice.Models.Services> Services { get; set; } = default!;
        public DbSet<Autoservice.Models.Orders> Orders { get; set; } = default!;
        public DbSet<Autoservice.Models.Categories> Categories { get; set; } = default!;
        public DbSet<Autoservice.Models.Order_service> Order_service { get; set; } = default!;
        public DbSet<Autoservice.Models.Cart> Cart { get; set; } = default!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Services>().ToTable("Services");
            modelBuilder.Entity<Clients>().ToTable("Clients");
            modelBuilder.Entity<Employees>().ToTable("Employees");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<Categories>().ToTable("Categories");
            modelBuilder.Entity<Order_service>().ToTable("Order_service");
            modelBuilder.Entity<Cart>().ToTable("Cart");

            modelBuilder.Entity<Categories>()
            .HasMany(e => e.Services)
            .WithOne(e => e.Categories)
            .HasForeignKey(e => e.category_id)
            .IsRequired();

            modelBuilder.Entity<Services>()
           .HasOne(e => e.Categories)
           .WithMany(e => e.Services)
           .HasForeignKey(e => e.category_id)
           .IsRequired();

            modelBuilder.Entity<Clients>()
           .HasMany(e => e.Orders)
           .WithOne(e => e.Clients)
           .HasForeignKey(e => e.client_id)
           .IsRequired();

            modelBuilder.Entity<Orders>()
           .HasOne(e => e.Clients)
           .WithMany(e => e.Orders)
           .HasForeignKey(e => e.client_id)
           .IsRequired();

            modelBuilder.Entity<Employees>()
           .HasMany(e => e.Orders)
           .WithOne(e => e.Employees)
           .HasForeignKey(e => e.employee_id)
           .IsRequired();

            modelBuilder.Entity<Orders>()
           .HasOne(e => e.Employees)
           .WithMany(e => e.Orders)
           .HasForeignKey(e => e.employee_id)
           .IsRequired();

            modelBuilder.Entity<Orders>()
            .HasMany(e => e.Order_service)
            .WithOne(e => e.Orders)
            .HasForeignKey(e => e.order_id)
            .IsRequired();

            modelBuilder.Entity<Order_service>()
            .HasOne(e => e.Orders)
            .WithMany(e => e.Order_service)
            .HasForeignKey(e => e.order_id)
            .IsRequired();

            modelBuilder.Entity<Services>()
            .HasMany(e => e.Order_service)
            .WithOne(e => e.Services)
            .HasForeignKey(e => e.service_id)
            .IsRequired();

            modelBuilder.Entity<Order_service>()
            .HasOne(e => e.Services)
            .WithMany(e => e.Order_service)
            .HasForeignKey(e => e.service_id)
            .IsRequired();

            modelBuilder.Entity<Services>()
            .HasMany(e => e.Cart)
            .WithOne(e => e.Services)
            .HasForeignKey(e => e.service_id)
            .IsRequired();

            modelBuilder.Entity<Cart>()
            .HasOne(e => e.Services)
            .WithMany(e => e.Cart)
            .HasForeignKey(e => e.service_id)
            .IsRequired();

            modelBuilder.Entity<Clients>()
            .HasMany(e => e.Cart)
            .WithOne(e => e.Clients)
            .HasForeignKey(e => e.client_id)
            .IsRequired();

            modelBuilder.Entity<Cart>()
            .HasOne(e => e.Clients)
            .WithMany(e => e.Cart)
            .HasForeignKey(e => e.client_id)
            .IsRequired();
        }

    }
}
