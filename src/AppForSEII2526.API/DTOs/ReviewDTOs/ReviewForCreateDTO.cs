namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    using Microsoft.CodeAnalysis;
    using NuGet.DependencyResolver;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO.Pipelines;
    using System.Linq;
    using System.Text.Json.Serialization;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class ReviewForCreateDTO
    {
        // solicita al cliente que introduzca el título de la reseña y el país desde donde se hace de forma obligatoria y de forma opcional el nombre del cliente.
        // Para cada dispositivo se pide un comentario y una puntuación de obligatoria.
        public ReviewForCreateDTO(string reviewTitle, string customerCountry,string? customerUserName,  IList<ReviewItemDTO> reviewItems)
        {
            ReviewTitle = reviewTitle ?? throw new ArgumentNullException(nameof(reviewTitle));
            CustomerCountry = customerCountry ?? throw new ArgumentNullException(nameof(customerCountry));

            CustomerUserName = customerUserName;
            
            ReviewItems = reviewItems ?? throw new ArgumentNullException(nameof(reviewItems));
        }

        public ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "El título debe tener entre 10 y 50 caracteres")]
        [Display(Name = "Título de la reseña")]
        public string ReviewTitle { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese pais")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El pais tiene que tener entre 3 y 30 caracteres")]
        [Display(Name = "Pais")]
        public string CustomerCountry { get; set; } = string.Empty;

        
        [EmailAddress]
        [Display(Name = "Nombre de usuario")]
        public string? CustomerUserName { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Artículos reseñados")]
        public IList<ReviewItemDTO> ReviewItems { get; set; }

        [Display(Name = "Puntuación promedio")]
        [JsonPropertyName("AverageRating")]
        public int AverageRating
        {
            get
            {
                return (int)(ReviewItems.Any() ? ReviewItems.Average(r => r.Rating) : 0);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is ReviewForCreateDTO dTO &&
                   ReviewTitle == dTO.ReviewTitle &&
                   CustomerCountry == dTO.CustomerCountry &&
                   CustomerUserName == dTO.CustomerUserName &&
                   EqualityComparer<IList<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems) &&
                   AverageRating == dTO.AverageRating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReviewTitle, CustomerCountry, CustomerUserName, ReviewItems, AverageRating);
        }
    }
}