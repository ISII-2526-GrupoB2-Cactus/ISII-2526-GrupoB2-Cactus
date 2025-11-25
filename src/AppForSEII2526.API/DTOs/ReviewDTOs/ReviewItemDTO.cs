
namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewItemDTO
    {
        //así como de cada dispositivo su nombre, modelo, año, puntuación y el comentario.
        public ReviewItemDTO(int id, string name, string model,  int year, int rating, string? comments)
        {
            Id = id;
            Name = name;
            Model = model;
            Year = year;
            Rating = rating;
            Comments = comments;
        }
        public ReviewItemDTO()
        {
        }
       

        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Model == dTO.Model &&
                   Year == dTO.Year &&
                   Rating == dTO.Rating &&
                   Comments == dTO.Comments;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Model, Year, Rating, Comments);
        }
    }
}
