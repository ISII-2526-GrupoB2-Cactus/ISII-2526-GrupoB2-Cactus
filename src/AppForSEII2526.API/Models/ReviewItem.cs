namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(ReviewId))]
    public class ReviewItem
    {
        public ReviewItem()
        {
        }
        public ReviewItem(int deviceId, int rating, string? comments = null)
        {
            DeviceId = deviceId;
            Rating = rating;
            Comments = comments;
        }

        public ReviewItem(string? comments, int deviceId, int rating, int reviewId)
        {
            Comments = comments;
            DeviceId = deviceId;
            Rating = rating;
            ReviewId = reviewId;
        }
        

        [StringLength(50, ErrorMessage = "Los comentarios no pueden tener mas de 50 caracteres")]
        public string? Comments { get; set; }

        [Required]
        public int DeviceId { get; set; }


        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [Required]
        public int ReviewId { get; set; }

        public Review Review { get; set; }

        public Device Device { get; set; }

        // Implementación del método Equals
        public override bool Equals(object? obj)
        {
            if (obj is ReviewItem other)
            {
                return DeviceId == other.DeviceId &&
                       ReviewId == other.ReviewId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, ReviewId);
        }
    }
}