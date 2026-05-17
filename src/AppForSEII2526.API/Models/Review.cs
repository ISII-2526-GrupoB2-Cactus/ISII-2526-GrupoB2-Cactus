
using System.Collections.Generic;

namespace AppForSEII2526.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Review
    {
        public Review()
        {
            ReviewItems = new List<ReviewItem>();
        }

        public Review(string customerId, DateTime dateOfReview, int overallRating, string reviewTitle,IList<ReviewItem> reviewItems, ApplicationUser user)
        {
            CustomerId = customerId;
            DateOfReview = dateOfReview;
            OverallRating = overallRating;
            ReviewTitle = reviewTitle;
            ReviewItems = reviewItems;
            ApplicationUser = user;
        }


        [Key]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre tiene que tener entre 10 y 50 caracteres", MinimumLength = 10)]
        public string CustomerId { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Review")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfReview { get; set; }

        [Range(1, 5, ErrorMessage = "La valoración global debe estar entre 1 y 5.")]
        public int OverallRating { get; set; }


        [StringLength(50, ErrorMessage = "El nombre tiene que tener entre 10 y 50 caracteres", MinimumLength = 10)]
        public string ReviewTitle { get; set; }

        public IList<ReviewItem> ReviewItems { get; set; } //Atributo para relacion

  
        public ApplicationUser ApplicationUser { get; set; } //Atributo con claser relacion ApplicationUser

        public override bool Equals(object? obj)
        {
            return obj is Review review &&
                   ReviewId == review.ReviewId &&
                   CustomerId == review.CustomerId &&
                   DateOfReview == review.DateOfReview &&
                   OverallRating == review.OverallRating &&
                   ReviewTitle == review.ReviewTitle &&
                   EqualityComparer<IList<ReviewItem>>.Default.Equals(ReviewItems, review.ReviewItems) &&
                   EqualityComparer<ApplicationUser>.Default.Equals(ApplicationUser, review.ApplicationUser);
        }
    }

}
