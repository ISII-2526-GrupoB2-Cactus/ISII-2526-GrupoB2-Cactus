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
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            NameSurname = nameSurname ?? throw new ArgumentNullException(nameof(nameSurname));
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

        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Añada su nombre y apellidos")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "El nombre y los apellidos deben tener al menos 10 caracteres")]
        public string NameSurname { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }



        public override bool Equals(object? obj)
        {
            return obj is PurchaseForCreateDTO dto &&
                   DeliveryAddress == dto.DeliveryAddress &&
                   PurchaseItems.SequenceEqual(dto.PurchaseItems) &&
                   TotalPrice == dto.TotalPrice &&
                   UserName == dto.UserName &&
                   NameSurname == dto.NameSurname &&
                   PaymentMethod == dto.PaymentMethod;
        }





    }
}
