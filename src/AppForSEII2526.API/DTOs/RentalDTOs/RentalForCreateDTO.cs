using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalForCreateDTO
    {
        public RentalForCreateDTO(string customerUserName, string customerNameSurname, string deliveryAddress, PaymentMethodType paymentMethod, DateTime rentalDateFrom, DateTime rentalDateTo, IList<RentalItemDTO> rentalItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PaymentMethod = paymentMethod;
            RentalDateFrom = rentalDateFrom;
            RentalDateTo = rentalDateTo;
            RentalItems = rentalItems ?? throw new ArgumentNullException(nameof(rentalItems));
        }

        public RentalForCreateDTO()
        {
            CustomerUserName = string.Empty;
            CustomerNameSurname = string.Empty;
            DeliveryAddress = string.Empty;
            RentalItems = new List<RentalItemDTO>();
        }

        public DateTime RentalDateFrom { get; set; }

        public DateTime RentalDateTo { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Dirección de entrega")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "La dirección debe tener al menos 10 caracteres")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese su dirección de entrega")]
        public string DeliveryAddress { get; set; }

        [EmailAddress]
        [Required]
        public string CustomerUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese su nombre y apellidos")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre y apellidos deben tener al menos 2 caracteres")]
        public string CustomerNameSurname { get; set; }

        public IList<RentalItemDTO> RentalItems { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        private int NumberOfDays
        {
            get
            {
                return (RentalDateTo - RentalDateFrom).Days;
            }
        }

        [Display(Name = "Precio Total")]
        public decimal TotalPrice
        {
            get
            {
                if (RentalItems == null) return 0;
                return RentalItems.Sum(ri => (decimal)ri.PriceForRent * ri.Quantity * NumberOfDays);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is RentalForCreateDTO dTO &&
                   CompareDate(RentalDateFrom, dTO.RentalDateFrom) &&
                   CompareDate(RentalDateTo, dTO.RentalDateTo) &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   CustomerUserName == dTO.CustomerUserName &&
                   CustomerNameSurname == dTO.CustomerNameSurname &&
                   RentalItems.SequenceEqual(dTO.RentalItems) &&
                   PaymentMethod == dTO.PaymentMethod &&
                   TotalPrice == dTO.TotalPrice;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CustomerUserName, CustomerNameSurname, DeliveryAddress,
                                   PaymentMethod, RentalDateFrom, RentalDateTo, RentalItems);
        }
    }
}