namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewDetailDTO : ReviewForCreateDTO
    {
        public ReviewDetailDTO(int id, string title, string customerCountry, string? customerUserName, DateTime reviewDate, IList<ReviewItemDTO> reviewItems)
            : base(title, customerCountry, customerUserName, reviewDate, reviewItems)
        {
            Id = id;
        }

        public int Id { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewDetailDTO dTO &&
                   base.Equals(obj) &&
                   Id == dTO.Id &&
                   ReviewDate == dTO.ReviewDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, ReviewDate);
        }
    }
}
