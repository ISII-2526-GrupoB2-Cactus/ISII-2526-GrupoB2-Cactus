namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewDetailDTO : ReviewForCreateDTO
    {
        public ReviewDetailDTO(int id,
                          DateTime reviewDate,
                          string customerUserName,
                          string customerNameSurname,
                          IList<ReviewItemDTO> reviewItems)
       : base(customerUserName,
              customerNameSurname,
              reviewDate,
              reviewItems)
        {
            Id = id;
            ReviewDate = reviewDate;
        }

        public int Id { get; set; }

        public DateTime ReviewDate { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewDetailDTO dto &&
                   base.Equals(obj) &&
                   Id == dto.Id &&
                   CompareDate(ReviewDate, dto.ReviewDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, ReviewDate);
        }
    }
}
