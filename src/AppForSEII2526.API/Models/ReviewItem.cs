
namespace AppForSEII2526.API.Models
{
    public class ReviewItem
    {


        public ReviewItem()
        {

        }
        public ReviewItem(int id, string comments, int rating, int reviewId, int deviceId)
        {
            Id = id;
            Comments = comments;
            Rating = rating;
            ReviewId = reviewId;
            DeviceId = deviceId;
        }

        [Key]
        public int Id { get; set; }
        

        [StringLength(100, ErrorMessage = "Los comentarios no pueden tener mas de 100 caracteres")] 
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
