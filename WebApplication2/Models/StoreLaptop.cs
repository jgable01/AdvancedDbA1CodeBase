using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class StoreLaptop
    {
        public Guid StoreLaptopId { get; set; }
        public Store Store { get; set; }

        public Guid LaptopId { get; set; }
        public Laptop Laptop { get; set; }

        [Range(-10, int.MaxValue, ErrorMessage = "Quantity can't be less than -10. Current quantity is {_quantity}.")]
        public int Quantity { get; set; }
    }
}
