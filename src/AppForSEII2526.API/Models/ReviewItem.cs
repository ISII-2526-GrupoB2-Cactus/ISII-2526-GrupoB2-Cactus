
namespace AppForSEII2526.API.Models
{
    public class ReviewItem
    {


        public ReviewItem()
        {

        }
        public ReviewItem(int deviceId, string comments, int id, int rating, int reviewId)
        {
            DeviceId = deviceId;
            Comments = comments;
            Id = id;
            Rating = rating;
            ReviewId = reviewId;
        }


        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        public string Comments { get; set; }

        public int Id { get; set; }
        public int Rating { get; set; }
        public int ReviewId { get; set; }
        public Device Device { get; set; } //Atributo para relacion
        public Review Review { get; set; } //Atributo para relacion
        public int DeviceId { get; set; }

    }

}
