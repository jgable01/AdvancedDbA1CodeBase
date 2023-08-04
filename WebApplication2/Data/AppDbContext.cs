using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        { }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreLaptop> StoreLaptops { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder) // Took inspiration from stack-overflow. This allowed me to maintain variable names in the database while still being primary keys
        {
            modelBuilder.Entity<Store>().HasKey(s => s.StoreNumber);
            modelBuilder.Entity<StoreLaptop>().HasKey(s => s.StoreLaptopId);

        }
    }
}
