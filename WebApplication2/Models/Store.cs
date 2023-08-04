namespace WebApplication2.Models
{
    public class Store
    {
        public Guid StoreNumber { get; set; }

        private string _streetName;

        public string StreetName
        {
            get => _streetName;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Street name must be at least three characters in length.");
                }
                _streetName = value;
            }
        }

        private string _province;

        public string Province
        {
            get => _province;
            set
            {
                // Assuming that you have a method to validate the province
                if (!IsValidCanadianProvince(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Province must be a valid Canadian province.");
                }
                _province = value;
            }
        }

        public ICollection<Laptop> Laptops { get; set; } = new HashSet<Laptop>();

        private bool IsValidCanadianProvince(string province)
        {
            // Add your logic here to validate the province
            // For simplicity, this always returns true
            return true;
        }
    }
}
