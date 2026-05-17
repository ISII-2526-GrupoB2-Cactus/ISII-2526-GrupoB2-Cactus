namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseItemDTO
    {
        public PurchaseItemDTO(int deviceID, string brand, string model, string color, decimal priceForPurchase, int quantity, string? description = null)
        {
            DeviceID = deviceID;
            Brand = brand ?? throw new ArgumentNullException(nameof(brand));
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Color = color ?? throw new ArgumentNullException(nameof(color));
            PriceForPurchase = priceForPurchase;
            Quantity = quantity;
            Description = description; // opcional
        }

        public int DeviceID { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        [StringLength(30)]
        public string Color { get; set; }

        [Display(Name = "Precio de la compra")]
        [Precision(10, 2)]
        public decimal PriceForPurchase { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Pon una canditad valida")]
        public int Quantity { get; set; }

        public string? Description { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PurchaseItemDTO model &&
                   DeviceID == model.DeviceID &&
                   Brand == model.Brand &&
                   Model == model.Model &&
                   Color == model.Color &&
                   PriceForPurchase == model.PriceForPurchase &&
                   Quantity == model.Quantity &&
                   Description == model.Description;
        }




    }
}