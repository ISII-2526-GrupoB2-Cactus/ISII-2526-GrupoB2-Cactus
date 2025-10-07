using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Models
{
    public class PurchaseItem
    {
        public PurchaseItem()
        {

        }

        public PurchaseItem(Purchase purchase, int quantity, Device device, string description)// preguntar si se mete por el constructor la descripcion
        {

            Purchase = purchase;
            Device = device;
            PurchaseId = purchase.Id;
            DeviceId = device.Id;
            Description = description;
            Price = device.PriceForPurchase;
            Quantity = quantity;
        }

        public Purchase Purchase { get; set; } //relacion con la clase Purchase

        [Key]
        public int PurchaseId { get; set; }

        public Device Device { get; set; } //relacion con la clase Device

        public int DeviceId { get; set; }


        public string? Description { get; set; }//puede ser nulo, no es obligatorio

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Precio de PurchaseItem")]
        public double Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tienes que seleccionar al menos 1 articulo")]
        public int Quantity { get; set; }






    }




}