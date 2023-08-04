namespace WebApplication2.Models
{
    public class StoreLaptop
    {
        public Guid StoreLaptopId { get; set; }
        public Store Store { get; set; }

        public Guid LaptopId { get; set; }
        public Laptop Laptop { get; set; }

        private int _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < -10)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Quantity can't be less than -10.");
                }
                _quantity = value;
            }
        }
    }
}
