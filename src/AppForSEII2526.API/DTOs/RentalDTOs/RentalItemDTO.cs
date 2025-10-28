namespace AppForSEII2526.API.DTOs.DeviceDTO
{
    public class DispositivoAlquilerItemDTO
    {
        public DispositivoAlquilerItemDTO(int deviceId, string name, string brand, string model, double priceForRent, int quantity = 1)
        {
            DeviceId = deviceId;
            Name = name;
            Brand = brand;
            Model = model;
            PriceForRent = priceForRent;
            Quantity = quantity;
        }

        public int DeviceId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public double PriceForRent { get; set; }

        public int Quantity { get; set; } = 1;

        public override bool Equals(object? obj)
        {
            return obj is DispositivoAlquilerItemDTO dTO &&
                   DeviceId == dTO.DeviceId &&
                   Name == dTO.Name &&
                   Brand == dTO.Brand &&
                   Model == dTO.Model &&
                   PriceForRent == dTO.PriceForRent &&
                   Quantity == dTO.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, Name, Brand, Model, PriceForRent, Quantity);
        }
    }
}

ksqdiq