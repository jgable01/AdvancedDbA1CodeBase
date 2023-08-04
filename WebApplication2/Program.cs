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

SeedData.Initialize(app.Services);


using (var serviceScope = app.Services.CreateScope())
{
    AppDbContext context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(serviceScope.ServiceProvider);
}

app.MapGet("/laptops/search", async (AppDbContext context, decimal? priceAbove, decimal? priceBelow,
    Guid? storeNumber, string? province, LaptopCondition? condition, int? brandId, string? searchPhrase) =>
{
    LaptopSearchRequest request = LaptopSearchRequest.Create(priceAbove, priceBelow, storeNumber, province, condition, brandId, searchPhrase);
    IQueryable<Laptop> query = context.Laptops.Include(l => l.Brand);

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

    if (request.StoreNumber.HasValue || !string.IsNullOrWhiteSpace(request.Province))
    {
        IQueryable<StoreLaptop> storeLaptopQuery = context.StoreLaptops.Where(sl => sl.Quantity > 0);

        if (request.StoreNumber.HasValue)
            storeLaptopQuery = storeLaptopQuery.Where(sl => sl.Store.StoreNumber == request.StoreNumber.Value);

        if (!string.IsNullOrWhiteSpace(request.Province))
            storeLaptopQuery = storeLaptopQuery.Where(sl => sl.Store.Province == request.Province);

        List<Guid> laptopIdsInStock = await storeLaptopQuery.Select(sl => sl.LaptopId).ToListAsync();
        query = query.Where(l => laptopIdsInStock.Contains(l.Id));
    }

    List<Laptop> laptops = await query.ToListAsync();

    return laptops.Select(l => new
    {
        l.Id,
        l.Model,
        l.Price,
        l.Condition,
        Brand = new { l.Brand.Id, l.Brand.Name }
    });
});

app.MapGet("/stores/{storeNumber}/inventory", async (AppDbContext context, Guid storeNumber) =>
{
    Store store = await context.Stores.FindAsync(storeNumber);
    if (store == null)
    {
        return Results.NotFound($"Store with number {storeNumber} not found.");
    }

    List<StoreLaptop> storeLaptops = context.StoreLaptops
        .Where(sl => sl.Store.StoreNumber == storeNumber && sl.Quantity > 0)
        .Include(sl => sl.Laptop).ThenInclude(l => l.Brand)
        .ToList();

    return Results.Ok(storeLaptops.Select(sl => new
    {
        Id = sl.Laptop.Id,
        Model = sl.Laptop.Model,
        Price = sl.Laptop.Price,
        Condition = sl.Laptop.Condition,
        Brand = new { Id = sl.Laptop.Brand.Id, Name = sl.Laptop.Brand.Name },
        Quantity = sl.Quantity
    }));
});

app.MapPost("/stores/{storeNumber}/{laptopId}/changeQuantity", async (AppDbContext context, Guid storeNumber, Guid laptopId, int amount) =>
{
    StoreLaptop storeLaptop = await context.StoreLaptops
        .Where(sl => sl.Store.StoreNumber == storeNumber && sl.LaptopId == laptopId)
        .FirstOrDefaultAsync();

    if (storeLaptop == null)
        return Results.NotFound();

    storeLaptop.Quantity += amount;
    await context.SaveChangesAsync();

    return Results.Ok("Quantity updated successfully.");
});

app.MapGet("/brands/{brandId}/averagePrice", async (AppDbContext context, int brandId) =>
{
    Brand brand = await context.Brands.FirstOrDefaultAsync(b => b.Id == brandId);

    if (brand == null)
        return Results.NotFound();

    // Make sure to load the laptops for the specific brand
    await context.Entry(brand).Collection(b => b.Laptops).LoadAsync();

    decimal averagePrice = brand.Laptops.Average(l => l.Price);

    var result = new
    {
        LaptopCount = brand.Laptops.Count,
        AveragePrice = averagePrice
    };

    return Results.Ok(result);
});

app.MapGet("/stores/groupedByProvince", async (AppDbContext context) =>
{
    var groupedStores = await context.Stores
        .GroupBy(s => s.Province)
        .Select(g => new
        {
            Province = g.Key,
            Stores = g.Select(s => new { s.StoreNumber, s.StreetName })
        })
        .ToListAsync();

    return Results.Ok(groupedStores);
});





app.Run();
