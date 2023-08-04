using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class LaptopSearchRequest // This is a DTO (Data Transfer Object). It's a class that's used to transfer data between different parts of the application. In this case, it's used to transfer data from the controller to the view
    {
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero.")] // This is a data annotation. It's used to add validation to a property. In this case, it's used to ensure that the user can't enter a negative number for the price
        public decimal? PriceAbove { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero.")] 
        public decimal? PriceBelow { get; set; }

        public Guid? StoreNumber { get; set; }

        public string? Province { get; set; }

        public LaptopCondition? Condition { get; set; }

        public int? BrandId { get; set; }

        public string? SearchPhrase { get; set; }

        public static LaptopSearchRequest Create(decimal? priceAbove, decimal? priceBelow,
        Guid? storeNumber, string? province, LaptopCondition? condition, int? brandId, string? searchPhrase)
        {
            // This is a factory method. It's a method that creates an instance of a class
            // It's useful because it allows us to create an instance of a class without having to call the constructor directly
            return new LaptopSearchRequest
            {
                PriceAbove = priceAbove,
                PriceBelow = priceBelow,
                StoreNumber = storeNumber,
                Province = province,
                Condition = condition,
                BrandId = brandId,
                SearchPhrase = searchPhrase
            };
        }
    }

}
