namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
    public class DeviceForPurchaseDTO
    {
        public DeviceForPurchaseDTO()
        {
        }

        // Constructor completo
        public DeviceForPurchaseDTO(
            int id,
            string brand,
            string name,
            string model,
            string color,
            decimal priceForPurchase,
            int quantityForPurchase,
            int year)
        {
            Id = id;
            Brand = brand ?? throw new ArgumentNullException(nameof(brand));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Color = color ?? throw new ArgumentNullException(nameof(color));
            PriceForPurchase =(decimal) priceForPurchase;
            QuantityForPurchase = quantityForPurchase;
            Year = year;
         
        }

        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La marca no puede superar los 50 caracteres.")]
        public string Brand { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El modelo no puede superar los 50 caracteres.")]
        public string Model { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El color no puede superar los 30 caracteres.")]
        public string Color { get; set; } = string.Empty;

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(1, float.MaxValue, ErrorMessage = "El precio mínimo de compra es 1.")]
        [Display(Name = "Precio de Compra")]
        [Precision(10, 2)]
        public decimal PriceForPurchase { get; set; }

        [Display(Name = "Cantidad Disponible")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int QuantityForPurchase { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "El año no puede ser negativo.")]
        public int Year { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is DeviceForPurchaseDTO dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Brand == dto.Brand &&
                   Model == dto.Model &&
                   Color == dto.Color &&
                   PriceForPurchase == dto.PriceForPurchase &&
                   QuantityForPurchase == dto.QuantityForPurchase;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }





    }
}
