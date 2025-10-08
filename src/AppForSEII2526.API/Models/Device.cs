namespace AppForSEII2526.API.Models
{
    public class Device
    {

        public enum QualityType
        {
            New,
            LikeNew,
            Used,
            Refurbished
        }


        public Device()
        {

        }

        public Device(int id, string brand, string color, string name, double priceForPurchase, double priceForRent, int year, QualityType quality, int quantityForPurchase, int quantityForRent)
        {
            Id = id;
            Brand = brand;
            Color = color;
            Name = name;
            PriceForPurchase = priceForPurchase;
            PriceForRent = priceForRent;
            Year = year;
            Quality = quality;
            QuantityForPurchase = quantityForPurchase;
            QuantityForRent = quantityForRent;
        }




        [Key]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "La marca no puede tener más de 50 caracteres")]
        public string Brand { get; set; }

        [StringLength(50, ErrorMessage = "La descripcion no puede tener más de 50 caracteres")]
        public string? Description { get; set; }


        [StringLength(50, ErrorMessage = "El color no puede tener más de 50 caracteres")]
        public string Color { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio minimo es 0.5 ")]
        [Display(Name = "Precio de compra")]
        [Precision(10, 2)]
        public double PriceForPurchase { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio minimo es 0.5 ")]
        [Display(Name = "Precio de alquiler")]
        [Precision(10, 2)]
        public double PriceForRent { get; set; }

        public int Year { get; set; }

        public QualityType Quality { get; set; } // preguntar si esto es una enumeracion que no han puesto 

        [Display(Name = "Cantidad de Compra")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad minima de compra es 1")]
        public int QuantityForPurchase { get; set; }

        [Display(Name = "Cantidad de Alquiler")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad minima de alquiler es 1")]
        public int QuantityForRent { get; set; }


        public IList<RentDevice> RentedDevices { get; set; } //las relaciones con cada una de las clases
        public IList<ReviewItem> ReviewItems { get; set; }
        public IList<PurchaseItem> PurchaseItems { get; set; }


        public Model Model { get; set; }






    }
}