namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.Json.Serialization;

    public class ReviewForCreateDTO
    {
        public ReviewForCreateDTO(string customerUserName, string customerNameSurname, DateTime reviewDate, string reviewTitle, IList<ReviewItemDTO> reviewItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            ReviewDate = reviewDate;
            ReviewTitle = reviewTitle ?? throw new ArgumentNullException(nameof(reviewTitle));
            ReviewItems = reviewItems ?? throw new ArgumentNullException(nameof(reviewItems));
        }

        public ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
            ReviewTitle = string.Empty;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Nombre de usuario")]
        public string CustomerUserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese su nombre y apellido")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre y apellido deben tener al menos 5 caracteres")]
        [Display(Name = "Nombre completo")]
        public string CustomerNameSurname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fecha de la reseña")]
        public DateTime ReviewDate { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "El título debe tener entre 10 y 50 caracteres")]
        [Display(Name = "Título de la reseña")]
        public string ReviewTitle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Artículos reseñados")]
        public IList<ReviewItemDTO> ReviewItems { get; set; }

        [Display(Name = "Puntuación promedio")]
        [JsonPropertyName("AverageRating")]
        public double AverageRating
        {
            get
            {
                return ReviewItems.Any() ? ReviewItems.Average(r => r.Rating) : 0;
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is ReviewForCreateDTO dto &&
                   CompareDate(ReviewDate, dto.ReviewDate) &&
                   CustomerUserName == dto.CustomerUserName &&
                   CustomerNameSurname == dto.CustomerNameSurname &&
                   ReviewTitle == dto.ReviewTitle &&
                   ReviewItems.SequenceEqual(dto.ReviewItems) &&
                   AverageRating == dto.AverageRating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CustomerUserName, CustomerNameSurname, ReviewDate, ReviewTitle, ReviewItems, AverageRating);
        }
    }
}