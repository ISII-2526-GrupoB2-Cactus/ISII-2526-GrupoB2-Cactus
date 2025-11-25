namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewDetailDTO
    {
        // El sistema muestra la reseña realizada, indicando los datos del cliente (nombre y país),el título de la reseña y la fecha en que se realizó
        public ReviewDetailDTO(string customerUserName, string customerCountry, string reviewTitle, DateTime reviewDate, IList<ReviewItemDTO> reviewItems)
        {
            CustomerUserName = customerUserName;
            CustomerCountry = customerCountry;
            ReviewTitle = reviewTitle;
            ReviewDate = reviewDate;
            ReviewItems = reviewItems;
        }

        public ReviewDetailDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        public int Id { get; set; }

        public DateTime ReviewDate { get; set; }

        public string CustomerUserName { get; set; } = string.Empty; 

        public string CustomerCountry { get; set; } = string.Empty;

        public string ReviewTitle { get; set; } = string.Empty;

        public IList<ReviewItemDTO> ReviewItems { get; set; }

        public double AverageRating
        {
            get
            {
                return ReviewItems.Any() ? ReviewItems.Average(r => r.Rating) : 0;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ReviewDetailDTO dTO &&
                   Id == dTO.Id &&
                   ReviewDate == dTO.ReviewDate &&
                   CustomerUserName == dTO.CustomerUserName &&
                   CustomerCountry == dTO.CustomerCountry &&
                   ReviewTitle == dTO.ReviewTitle &&
                   EqualityComparer<IList<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems) &&
                   AverageRating == dTO.AverageRating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ReviewDate, CustomerUserName, CustomerCountry, ReviewTitle, ReviewItems, AverageRating);
        }
    }
}
