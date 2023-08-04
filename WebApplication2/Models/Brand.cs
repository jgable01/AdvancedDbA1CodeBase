using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Brand name must be at least three characters in length.")]
        public string Name { get; set; }

        public HashSet<Laptop> Laptops { get; set; } = new HashSet<Laptop>();
    }
}
