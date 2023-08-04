using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class LaptopSearchRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero.")]
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
