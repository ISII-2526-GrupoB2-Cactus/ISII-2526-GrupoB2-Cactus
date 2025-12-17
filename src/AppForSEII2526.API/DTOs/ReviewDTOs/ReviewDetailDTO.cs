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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ReviewDetailDTO)obj;

            return Id == other.Id &&
                   CustomerUserName == other.CustomerUserName &&
                   CustomerCountry == other.CustomerCountry &&
                   ReviewTitle == other.ReviewTitle &&
                   ReviewDate.Date == other.ReviewDate.Date &&
                   ReviewItems.Count == other.ReviewItems.Count &&
                   ReviewItems.All(item => other.ReviewItems.Contains(item));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, CustomerUserName, CustomerCountry, ReviewTitle, ReviewDate);
        }
    }
}
