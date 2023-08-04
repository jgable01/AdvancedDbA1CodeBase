using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Data;
using WebApplication2.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LaptopStore"));
});


var app = builder.Build();

SeedData.Initialize(app.Services); // Seed the data


using (var serviceScope = app.Services.CreateScope()) // scope for the service provider so we can get the AppDbContext
{
    AppDbContext context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(serviceScope.ServiceProvider);
}


// Endpoints

// Get all laptops, with optional query parameters -- I referenced stackoverflow for portions of this endpoint, but I did not copy and paste code and provided comments to show my understanding of the code
app.MapGet("/laptops/search", async (AppDbContext context, decimal? priceAbove, decimal? priceBelow,
    Guid? storeNumber, string? province, LaptopCondition? condition, int? brandId, string? searchPhrase) => 
{
    LaptopSearchRequest request = LaptopSearchRequest.Create(priceAbove, priceBelow, storeNumber, province, condition, brandId, searchPhrase); // Created laptopsearchrequest because we need to validate the request, this cleaned up the code a bit and made it easier to read
    IQueryable<Laptop> query = context.Laptops.Include(l => l.Brand);

    // Filter the query based on the request, if the request has a value for a property, filter the query by that property
    // This is a bit of a mess, but it's the best I could do without changing the request class and making it more complicated and less readable

    if (request.PriceAbove.HasValue)
        query = query.Where(l => l.Price >= request.PriceAbove.Value);
    if (request.PriceBelow.HasValue)
        query = query.Where(l => l.Price <= request.PriceBelow.Value);
    if (request.Condition.HasValue)
        query = query.Where(l => l.Condition == request.Condition.Value);
    if (request.BrandId.HasValue)
        query = query.Where(l => l.BrandId == request.BrandId.Value);
    if (!string.IsNullOrWhiteSpace(request.SearchPhrase))
        query = query.Where(l => l.Model.Contains(request.SearchPhrase));

    // If the request has a store number or province, filter the query by that store number or province

    if (request.StoreNumber.HasValue || !string.IsNullOrWhiteSpace(request.Province))
    {
        IQueryable<StoreLaptop> storeLaptopQuery = context.StoreLaptops.Where(sl => sl.Quantity > 0); // Changed this to a queryable because we need to add more where clauses to it

        if (request.StoreNumber.HasValue) 
            storeLaptopQuery = storeLaptopQuery.Where(sl => sl.Store.StoreNumber == request.StoreNumber.Value);

        if (!string.IsNullOrWhiteSpace(request.Province))
            storeLaptopQuery = storeLaptopQuery.Where(sl => sl.Store.Province == request.Province);

        List<Guid> laptopIdsInStock = await storeLaptopQuery.Select(sl => sl.LaptopId).ToListAsync(); // Changed this to a list because we need to use it in the return statement
        query = query.Where(l => laptopIdsInStock.Contains(l.Id));
    }

    List<Laptop> laptops = await query.ToListAsync();

    return laptops.Select(l => new // Changed this to a list because we need to use it in the return statement
    {
        l.Id,
        l.Model,
        l.Price,
        l.Condition,
        Brand = new { l.Brand.Id, l.Brand.Name }
    });
});

// Get laptops from a specific store
app.MapGet("/stores/{storeNumber}/inventory", async (AppDbContext context, Guid storeNumber) =>
{
    Store store = await context.Stores.FindAsync(storeNumber);
    if (store == null)
    {
        return Results.NotFound($"Store with number {storeNumber} not found.");
    }

    List<StoreLaptop> storeLaptops = context.StoreLaptops // Changed this to a list because we need to use it in the return statement
        .Where(sl => sl.Store.StoreNumber == storeNumber && sl.Quantity > 0)
        .Include(sl => sl.Laptop).ThenInclude(l => l.Brand)
        .ToList();

    return Results.Ok(storeLaptops.Select(sl => new // Same as above, changed this to a list because we need to use it in the return statement
    {
        Id = sl.Laptop.Id,
        Model = sl.Laptop.Model,
        Price = sl.Laptop.Price,
        Condition = sl.Laptop.Condition,
        Brand = new { Id = sl.Laptop.Brand.Id, Name = sl.Laptop.Brand.Name },
        Quantity = sl.Quantity
    }));
});

// Change the quantity of a laptop in a store. The amount parameter can be positive or negative and will be added (does not set) to the current quantity
app.MapPost("/stores/{storeNumber}/{laptopId}/changeQuantity", async (AppDbContext context, Guid storeNumber, Guid laptopId, int amount) =>
{
    StoreLaptop storeLaptop = await context.StoreLaptops // Changed this to a single or default because we only need one storelaptop
        .Where(sl => sl.Store.StoreNumber == storeNumber && sl.LaptopId == laptopId)
        .FirstOrDefaultAsync();

    if (storeLaptop == null)
        return Results.NotFound();

    storeLaptop.Quantity += amount;
    await context.SaveChangesAsync();

    return Results.Ok("Quantity updated successfully.");
});

// Get the average price of laptops for a specific brand
app.MapGet("/brands/{brandId}/averagePrice", async (AppDbContext context, int brandId) =>
{
    Brand brand = await context.Brands.FirstOrDefaultAsync(b => b.Id == brandId); // Changed this to a first or default because we only need one brand

    if (brand == null)
        return Results.NotFound();

    // Make sure to load the laptops for the specific brand
    await context.Entry(brand).Collection(b => b.Laptops).LoadAsync();

    decimal averagePrice = brand.Laptops.Average(l => l.Price);

    var result = new // I used var here because the type is anonymous and I don't want to write it out lol, sorry!
    {
        LaptopCount = brand.Laptops.Count,
        AveragePrice = averagePrice
    };

    return Results.Ok(result);
});

// Get all stores grouped by province
app.MapGet("/stores/groupedByProvince", async (AppDbContext context) =>
{
    var result = await context.Stores // I used var here because the type is anonymous and I don't want to write it out lol, sorry!
        .GroupBy(s => s.Province)
        .Select(g => new
        {
            Province = g.Key,
            Stores = g.Select(s => new { s.StoreNumber, s.StreetName })
        })
        .ToListAsync();

    return Results.Ok(result);
});



app.Run();
