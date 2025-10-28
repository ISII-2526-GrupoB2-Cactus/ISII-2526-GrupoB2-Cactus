namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewItemDTO
    {
        public ReviewItemDTO(int deviceId, int rating, string comments = "")
        {
            DeviceId = deviceId;
            Rating = rating;
            Comments = comments;
        }

        public int DeviceId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Rating { get; set; }

        [StringLength(100, ErrorMessage = "Los comentarios no pueden tener más de 100 caracteres")]
        public string? Comments { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDTO dto &&
                   DeviceId == dto.DeviceId &&
                   Rating == dto.Rating &&
                   Comments == dto.Comments;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, Rating, Comments);
        }
    }
}
