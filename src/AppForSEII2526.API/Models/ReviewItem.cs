
namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(ReviewId))]
    public class ReviewItem
    {


        public ReviewItem()
        {

        }
        public ReviewItem(string? comments, int rating, int reviewId, int deviceId)
        {
            Comments = comments;
            Rating = rating;
            ReviewId = reviewId;
            DeviceId = deviceId;
        }


        [StringLength(50, ErrorMessage = "Los comentarios no pueden tener mas de 50 caracteres")] 
        public string? Comments { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [Required]
        public int ReviewId { get; set; }
        public Review Review { get; set; } //Atributo para relacion con la clase Review

        [Required]
        public int DeviceId { get; set; }
        public Device Device { get; set; } //Atributo para relacion
        
        

    }

}
