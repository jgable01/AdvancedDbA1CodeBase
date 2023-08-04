using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Store
    {
        public Guid StoreNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Street name must be at least three characters in length.")]
        public string StreetName { get; set; }

        [Required]
        public string Province { get; set; }

        public ICollection<Laptop> Laptops { get; set; } = new HashSet<Laptop>();
    }
}
