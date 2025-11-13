using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseForCreateDTO
    {
        public PurchaseForCreateDTO()
        {
            PurchaseItems = new List<PurchaseItemDTO>();
        }

        public PurchaseForCreateDTO(string deliveryAddress, IList<PurchaseItemDTO> purchaseItems,
                                    string userName, string nameSurname, PaymentMethod paymentMethod)
        {
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PurchaseItems = purchaseItems ?? throw new ArgumentNullException(nameof(purchaseItems));
            CustomerName = userName ?? throw new ArgumentNullException(nameof(userName));
            CustomerSurname = nameSurname ?? throw new ArgumentNullException(nameof(nameSurname));
            PaymentMethod = paymentMethod;
        }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor añade tu direcion de envio.")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Debe tener al menos 10 caracteres.")]
        [Display(Name = "Direccion de envio")]
        public string DeliveryAddress { get; set; }

        public IList<PurchaseItemDTO> PurchaseItems { get; set; }

        [Display(Name = "Precio total")]
        public decimal TotalPrice
        {
            get
            {
                return PurchaseItems.Sum(pi => pi.Quantity * pi.PriceForPurchase);
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Añada su nombre.")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        public string CustomerName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Añada su apellido.")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "El apellido debe tener al menos 2 caracteres.")]
        public string CustomerSurname { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }



        public override bool Equals(object? obj)
        {
            return obj is PurchaseForCreateDTO dto &&
                   DeliveryAddress == dto.DeliveryAddress &&
                   PurchaseItems.SequenceEqual(dto.PurchaseItems) &&
                   TotalPrice == dto.TotalPrice &&
                   CustomerName == dto.CustomerName &&
                   CustomerSurname == dto.CustomerSurname &&
                   PaymentMethod == dto.PaymentMethod;
        }





    }
}