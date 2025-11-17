namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    
     public class ReviewDetailDTO
 {
     public ReviewDetailDTO(int id,
                       DateTime reviewDate,
                       string customerUserName,
                       string customerNameSurname,
                       IList<ReviewItemDTO> reviewItems)
     {
         Id = id;
         ReviewDate = reviewDate;
         CustomerUserName = customerUserName;
         CustomerNameSurname = customerNameSurname;
         ReviewItems = reviewItems;
     } //nuevo dto

     public ReviewDetailDTO()
     {
         ReviewItems = new List<ReviewItemDTO>();
     }

     public int Id { get; set; }

     public DateTime ReviewDate { get; set; }

     public string CustomerUserName { get; set; } = string.Empty;

     public string CustomerNameSurname { get; set; } = string.Empty;

     public IList<ReviewItemDTO> ReviewItems { get; set; }

     // Propiedad calculada para la puntuación promedio
     public double AverageRating
     {
         get
         {
             return ReviewItems.Any() ? ReviewItems.Average(r => r.Rating) : 0;
         }
     }

     private bool CompareDate(DateTime date1, DateTime date2)
     {
         return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
     }

     public override bool Equals(object? obj)
     {
         return obj is ReviewDetailDTO dto &&
                CompareDate(ReviewDate, dto.ReviewDate) &&
                Id == dto.Id &&
                CustomerUserName == dto.CustomerUserName &&
                CustomerNameSurname == dto.CustomerNameSurname &&
                ReviewItems.SequenceEqual(dto.ReviewItems) &&
                AverageRating == dto.AverageRating;
     }

     public override int GetHashCode()
     {
         return HashCode.Combine(Id, ReviewDate, CustomerUserName, CustomerNameSurname, ReviewItems, AverageRating);
     }
 }
}
