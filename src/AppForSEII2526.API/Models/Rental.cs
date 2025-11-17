namespace AppForSEII2526.API.Models
{
    //---------------------------------------------------------------
    //MÉTODOS DE LAS RESTRICCIONES
    //---------------------------------------------------------------

    //Método de PaymentMethod
    public enum PaymentMethodType
    {
        CreditCard,    // Tarjeta de crédito
        PayPal,        // PayPal
        Cash           // Efectivo
    }


    public class Rental
    {
        //---------------------------------------------------------------
        //CONSTRUCTORES
        //---------------------------------------------------------------

        public Rental()
        {
        }

        public Rental(string deliveryAddress, string nameCustomer, string surnameCustomer, decimal totalPrice, DateTime rentalDate, DateTime rentalDateFrom, DateTime rentalDateTo, PaymentMethodType paymentMethod)
        {
            DeliveryAddress = deliveryAddress;
            NameCustomer = nameCustomer;
            SurnameCustomer = surnameCustomer;
            TotalPrice = totalPrice;
            RentalDate = rentalDate;
            RentalDateFrom = rentalDateFrom;
            RentalDateTo = rentalDateTo;
            PaymentMethod = paymentMethod;
        }


        //---------------------------------------------------------------
        //ATRIBUTOS Y RESTRICCIONES
        //---------------------------------------------------------------

        public int Id { get; set; } //Primary key

        [StringLength(50, ErrorMessage = "La direccion debe tener entre 10 y 50 caracteres", MinimumLength = 10)]
        public string DeliveryAddress { get; set; }

        [StringLength(50, ErrorMessage = "El nombre del cliente debe tener entre 2 y 50 caracteres", MinimumLength = 2)]
        public string NameCustomer { get; set; }

        [StringLength(50, ErrorMessage = "El apellido del cliente debe tener entre 2 y 50 caracteres", MinimumLength = 2)]
        public string SurnameCustomer { get; set; }

        [Range(0.01, 9999999.99, ErrorMessage = "El costo debe estar entre 0.01 y 9,999,999.99")]
        [Precision(10, 2)]
        public decimal TotalPrice { get; set; }


        public DateTime RentalDate { get; set; }

        public DateTime RentalDateFrom { get; set; }

        public DateTime RentalDateTo { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        [EnumDataType(typeof(PaymentMethodType), ErrorMessage = "Método de pago no válido. Solo se permiten: Tarjeta de crédito, PayPal o Efectivo")]
        public PaymentMethodType PaymentMethod { get; set; }
        


        //---------------------------------------------------------------
        //RELACIONES
        //---------------------------------------------------------------

        // Relación uno-a-muchos: Un Rental puede tener múltiples RentalItems
        public IList<RentDevice> RentDevice { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        //---------------------------------------------------------------
        //MÉTODOS DE LA CLASE
        //---------------------------------------------------------------

    }
}