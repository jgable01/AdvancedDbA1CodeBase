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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships and constraints here
            // For instance, you might need to set up a many-to-many relationship between Stores and Laptops

            // This is just a placeholder; you'll need to define this according to your app requirements
            // modelBuilder.Entity<Store>().HasMany(s => s.Laptops).WithMany(l => l.Stores);
            modelBuilder.Entity<Store>().HasKey(s => s.StoreNumber);
            modelBuilder.Entity<StoreLaptop>().HasKey(s => s.StoreLaptopId);

        }
    }
}
