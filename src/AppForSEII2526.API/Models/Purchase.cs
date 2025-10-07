using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Models
{

    public enum PaymentMethod
    {
        CreditCard,
        PayPal
    }

    public class Purchase
    {

        public Purchase()
        {
            PurchaseItems = new List<PurchaseItem>();
        }




        public Purchase(int id, string customerUserName, string customerUserSurname, string deliveryAddress, DateTime purchaseDate, double totalPrice, int totalQuantity, PaymentMethod paymentMethod, IList<PurchaseItem> purchaseItems)
        {
            Id = id;
            CustomerUserName = customerUserName;
            CustomerUserSurname = customerUserSurname;
            DeliveryAddress = deliveryAddress;
            PurchaseDate = purchaseDate;
            TotalPrice = totalPrice;
            TotalQuantity = totalQuantity;
            PaymentMethod = paymentMethod;
            PurchaseItems = purchaseItems;
        }


        [Key]
        public int Id { get; set; }

        [Required, StringLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres")]
        public string CustomerUserName { get; set; }

        [Required, StringLength(30, ErrorMessage = "El apellido no puede tener más de 30 caracteres")]
        public string CustomerUserSurname { get; set; }

        [Required, StringLength(50, ErrorMessage = "La direccion no puede tener más de 50 caracteres")]
        public string DeliveryAddress { get; set; }

        public DateTime PurchaseDate { get; set; }

        [Required, DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(10, 2)]
        [Display(Name = "Precio de Compra")]
        public double TotalPrice { get; set; }

        [Required, Display(Name = "Cantidad de Compra")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad mínima de compra es 1")]
        public int TotalQuantity { get; set; }

        [Required, Display(Name = "Método de Pago")]
        public PaymentMethod PaymentMethod { get; set; }

        public IList<PurchaseItem> PurchaseItems { get; set; } //esto es lo que me genera la relacion






    }



}