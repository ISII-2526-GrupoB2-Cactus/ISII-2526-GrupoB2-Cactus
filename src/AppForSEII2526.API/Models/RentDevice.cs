namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(RentId))]
    public class RentDevice
    {

        //-----------------------------------
        //CONSTRUCTORES
        //-----------------------------------

        public RentDevice()
        {
        }

        public RentDevice(int rentId, int deviceId, double price, int quantity)
        {
            RentId = rentId;
            DeviceId = deviceId;
            Price = price;
            Quantity = quantity;
        }

        public RentDevice(int id, Rental rental, double priceForRent, int quantity)
        {
            Id = id;
            Rental = rental;
            PriceForRent = priceForRent;
            Quantity = quantity;
        }

        //-----------------------------------
        //ATRIBUTOS
        //-----------------------------------

        public int RentId { get; set; } //Primary Key

        [Required(ErrorMessage = "El ID del dispositivo es obligatorio")]
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        [Precision(10, 2)]
        public double Price { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; }



        //-----------------------------------
        //RELACIONES
        //-----------------------------------


        public Rental Rental { get; set; }

        public Device Device { get; set; }
        public int Id { get; }
        public double PriceForRent { get; }
    }
}
