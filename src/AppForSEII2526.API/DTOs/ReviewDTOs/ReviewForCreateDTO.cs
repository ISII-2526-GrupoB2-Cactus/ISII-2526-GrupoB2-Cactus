namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.Json.Serialization;
    public class ReviewForCreateDTO
    {
        public ReviewForCreateDTO(string customerUserName, string customerNameSurname, DateTime reviewDate, IList<ReviewItemDTO> reviewItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            ReviewDate = reviewDate;
            ReviewItems = reviewItems ?? throw new ArgumentNullException(nameof(reviewItems));
        }

        public ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        [Required]
        [EmailAddress]
        public string CustomerUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese su nombre y apellido")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre y apellido deben tener al menos 5 caracteres")]
        public string CustomerNameSurname { get; set; }

        [Required]
        [Display(Name = "Fecha de la reseña")]
        public DateTime ReviewDate { get; set; }

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
                   ReviewItems.SequenceEqual(dto.ReviewItems) &&
                   AverageRating == dto.AverageRating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CustomerUserName, CustomerNameSurname, ReviewDate, ReviewItems, AverageRating);
        }
    }
}
