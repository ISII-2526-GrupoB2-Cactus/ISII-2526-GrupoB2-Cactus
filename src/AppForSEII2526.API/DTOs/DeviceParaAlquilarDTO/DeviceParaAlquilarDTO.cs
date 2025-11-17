namespace AppForSEII2526.API.DTOs.DeviceDTO
{
    public class DeviceParaAlquilarDTO
    {

        //CONSTRUCTOR
        

        public DeviceParaAlquilarDTO(int id, string name, string model, string brand, int year, string color, double priceForRent)
        {
            Id = id;
            Name = name;
            Model = model;
            Brand = brand;
            Year = year;
            Color = color;
            PriceForRent = priceForRent;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; }


        [Required, StringLength(50, ErrorMessage = "El modelo no puede tener más de 50 caracteres")]
        public string Model { get; }


        [Required]
        [StringLength(50, ErrorMessage = "La marca no puede tener más de 50 caracteres")]
        public string Brand { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El color no puede tener más de 50 caracteres")]
        public string Color { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0.5, float.MaxValue, ErrorMessage = "El precio minimo es 0.5 ")]
        [Display(Name = "Precio de alquiler")]
        [Precision(10, 2)]
        public double PriceForRent { get; set; }


        public override bool Equals(object obj)
        {
            return obj is DeviceParaAlquilarDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Model == dTO.Model &&
                   Brand == dTO.Brand &&
                   Year == dTO.Year &&
                   Color == dTO.Color &&
                   PriceForRent == dTO.PriceForRent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Model, Brand, Year, Color, PriceForRent);
        }


    }
}


