namespace AppForSEII2526.API.DTOs.DeviceDTo
{
    public class DeviceForReviewDTO
    {
        public DeviceForReviewDTO(int id, string brand, string name, int year, string model, string color)
        {
            Id = id;
            Brand = brand;
            Name = name;
            Year = year;
            Model = model;
            Color = color;
        }

        public DeviceForReviewDTO() { }

        public int Id { get; set; }

        [Required, StringLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50, ErrorMessage = "La marca no puede tener más de 50 caracteres")]
        public string Brand { get; set; } = string.Empty;

        // int no nullable → no necesita [Required]
        public int Year { get; set; }

        [Required, StringLength(50, ErrorMessage = "El modelo no puede tener más de 50 caracteres")]
        public string Model { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El color no puede tener más de 50 caracteres")]
        public string Color { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            return obj is DeviceForReviewDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Brand == dTO.Brand &&
                   Year == dTO.Year &&
                   Model == dTO.Model &&
                   Color == dTO.Color;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Brand, Year, Model, Color);
        }
    }

}





