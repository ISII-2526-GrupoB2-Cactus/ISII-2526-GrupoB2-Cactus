
namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewDetailDTO : ReviewForCreateDTO
    {
        // El sistema muestra la reseña realizada, indicando los datos del cliente (nombre y país),el título de la reseña y la fecha en que se realizó, 
        //así como de cada dispositivo su nombre, modelo, año, puntuación y el comentario

        // Constructor
        public ReviewDetailDTO(int id, string customerCountry, string? customerUserName,string title, DateTime reviewDate, IList<ReviewItemDTO> reviewItems)
            : base(title, customerCountry, customerUserName, reviewDate, reviewItems)
        {
            Id = id;
            ReviewDate = reviewDate;
        }

        // Id de la reseña
        public int Id { get; set; }

        public DateTime ReviewDate { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewDetailDTO dTO &&
                   base.Equals(obj) &&
                   Title == dTO.Title &&
                   CustomerCountry == dTO.CustomerCountry &&
                   CustomerUserName == dTO.CustomerUserName &&
                   EqualityComparer<IList<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems) &&
                   ReviewDate == dTO.ReviewDate &&
                   AverageRating == dTO.AverageRating &&
                   Id == dTO.Id &&
                   ReviewDate == dTO.ReviewDate;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(Title);
            hash.Add(CustomerCountry);
            hash.Add(CustomerUserName);
            hash.Add(ReviewItems);
            hash.Add(ReviewDate);
            hash.Add(AverageRating);
            hash.Add(Id);
            hash.Add(ReviewDate);
            return hash.ToHashCode();
        }
    }
}