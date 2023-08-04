using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Laptop
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Laptop model name must be at least three characters in length.")]
        public string Model { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be less than zero.")]
        public decimal Price { get; set; }

        public LaptopCondition Condition { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }
    }

        public enum LaptopCondition
    {
        New,
        Refurbished,
        Rental
    }
}
