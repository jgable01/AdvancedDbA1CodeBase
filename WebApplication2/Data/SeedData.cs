using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebApplication2.Data;
using WebApplication2.Models;

public static class SeedData 
{
    public static void Initialize(IServiceProvider serviceProvider) // I used chatGPT as a reference for this, but I did not copy and paste the code and provided comments to show my understanding of the code
    {
        using (var serviceScope = serviceProvider.CreateScope()) // Create a scope for the service provider
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(); // Get the AppDbContext from the service provider

            // If the database exists, delete it and recreate it with the seed data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Check if any data exists, if so, the DB has been seeded
            if (context.Brands.Any() || context.Laptops.Any() || context.Stores.Any() || context.StoreLaptops.Any())
            {
                return;
            }

            // Otherwise, seed the data
            Brand[] brands = SeedBrands(context);
            Laptop[] laptops = SeedLaptops(context, brands);
            Store[] stores = SeedStores(context);
            SeedStoreLaptops(context, stores, laptops);

            // Assign laptops to stores
        }
    }

    private static Brand[] SeedBrands(AppDbContext context)
    {
        Brand[] brands = new Brand[]
        {
        new Brand { Name = "Dell" },
        new Brand { Name = "Lenovo" },
        new Brand { Name = "Alienware" }
        };

        context.Brands.AddRange(brands);
        context.SaveChanges();

        return brands;
    }

    private static Laptop[] SeedLaptops(AppDbContext context, Brand[] brands)
    {
        Laptop[] laptops = new Laptop[]
        {
        new Laptop { Model = "Laptop1", Price = 1000, Quantity = 10, Condition = LaptopCondition.New, Brand = brands[0] },
        new Laptop { Model = "Laptop 2", Price = 2000, Quantity = 20, Condition = LaptopCondition.Refurbished, Brand = brands[1] },
        new Laptop { Model = "Laptop 3", Price = 3000, Quantity = 30, Condition = LaptopCondition.Rental, Brand = brands[2] }
        };

        context.Laptops.AddRange(laptops);
        context.SaveChanges();

        return laptops;
    }

    private static Store[] SeedStores(AppDbContext context)
    {
        Store[] stores = new Store[]
        {
        new Store { StreetName = "Street 1", Province = "ON" },
        new Store { StreetName = "Street 2", Province = "BC" },
        new Store { StreetName = "Street 3", Province = "QC" }
        };

        context.Stores.AddRange(stores);
        context.SaveChanges();

        return stores;
    }

    private static void SeedStoreLaptops(AppDbContext context, Store[] stores, Laptop[] laptops)
    {
        StoreLaptop[] storeLaptops = new StoreLaptop[]
        {
        new StoreLaptop { Store = stores[0], Laptop = laptops[0], Quantity = 5 },
        new StoreLaptop { Store = stores[1], Laptop = laptops[1], Quantity = 10 },
        new StoreLaptop { Store = stores[2], Laptop = laptops[2], Quantity = 15 }
        };

        context.StoreLaptops.AddRange(storeLaptops);
        context.SaveChanges();
    }





}
