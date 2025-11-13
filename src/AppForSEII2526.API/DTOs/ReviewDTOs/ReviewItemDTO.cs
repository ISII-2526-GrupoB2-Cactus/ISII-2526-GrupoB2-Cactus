using System.ComponentModel.DataAnnotations;
namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewItemDTO
    {
        //Asi como de cada dispositivo su nombre, modelo, año, puntuación y el comentario
        public ReviewItemDTO() { }
        public ReviewItemDTO(int deviceId,int rating, string? comments)
        {
            //En este constructor estan los datos OBLIGATORIOS
            DeviceId = deviceId;
            Rating = rating;
            Comments = comments;
        }
        public ReviewItemDTO(int deviceId, string name, string model, int year, int rating, string? comments)
        {
            //En este constructor estan todos los datos QUE SE MUESTRAN DEL DISPOSITIVO
            DeviceId = deviceId;
            Name = name;
            Model = model;
            Year = year;
            Rating = rating;
            Comments = comments;
        }
       
        public int DeviceId { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        public string Name { get; set; }

        public string Model { get; set; }
        public int Year { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [StringLength(100, ErrorMessage = "Los comentarios no pueden tener más de 100 caracteres")]
        public string? Comments { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDTO dTO &&
                   DeviceId == dTO.DeviceId &&
                   Name == dTO.Name &&
                   Model == dTO.Model &&
                   Year == dTO.Year &&
                   Rating == dTO.Rating &&
                   Comments == dTO.Comments;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, Name, Model, Year, Rating, Comments);
        }
    }
}
