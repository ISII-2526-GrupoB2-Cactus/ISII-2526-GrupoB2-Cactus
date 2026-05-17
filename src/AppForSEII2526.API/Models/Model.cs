namespace AppForSEII2526.API.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Name { get; set; }

        public IList<Device> Devices { get; set; }

    }
}