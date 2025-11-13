using Microsoft.CodeAnalysis;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewForCreateDTO
    {
        //solicita al cliente que introduzca el título de la reseña y el país desde donde se hace de forma obligatoria y de forma opcional el nombre del cliente.
        //Para cada dispositivo se pide un comentario y una puntuación de obligatoria
        public ReviewForCreateDTO(string title, string customerCountry, string? customerUserName, IList<ReviewItemDTO> reviewItems)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            CustomerCountry = customerCountry ?? throw new ArgumentNullException(nameof(customerCountry));
            CustomerUserName = customerUserName; //Opcional
            ReviewItems = reviewItems ?? throw new ArgumentNullException(nameof(reviewItems));
        }

        //CONSTRUCTOR PARA ReviewDetailDTO
        public ReviewForCreateDTO(string title, string customerCountry, string? customerUserName, DateTime reviewDate, IList<ReviewItemDTO> reviewItems)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            CustomerCountry = customerCountry ?? throw new ArgumentNullException(nameof(customerCountry));
            CustomerUserName = customerUserName;
            ReviewDate = reviewDate;
        
        }

        public ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "El título debe tener entre 10 y 50 caracteres")]
        [Display(Name = "Título de la reseña")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El país debe tener entre 3 y 30 caracteres")]
        [Display(Name = "País del cliente")]
        public string CustomerCountry { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El nombre del cliente no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre del cliente (opcional)")] //Que el cliente introduzca su nombre es Opcional
        public string? CustomerUserName { get; set; }

        [Required]
        [Display(Name = "Dispositivos reseñados")]
        public IList<ReviewItemDTO> ReviewItems { get; set; }


        public DateTime ReviewDate { get; set; } //Lo necesito para ReviewDetailDTO

        // Promedio de puntuaciones
        [Display(Name = "Puntuación promedio")]
        [JsonPropertyName("AverageRating")]
        public double AverageRating => ReviewItems?.Any() == true ? ReviewItems.Average(r => r.Rating) : 0;

        public override bool Equals(object? obj)
        {
            return obj is ReviewForCreateDTO dTO &&
                   Title == dTO.Title &&
                   CustomerCountry == dTO.CustomerCountry &&
                   CustomerUserName == dTO.CustomerUserName &&
                   EqualityComparer<IList<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems) &&
                   AverageRating == dTO.AverageRating;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Title, CustomerCountry, CustomerUserName, ReviewItems, AverageRating);
        }
    }
}
