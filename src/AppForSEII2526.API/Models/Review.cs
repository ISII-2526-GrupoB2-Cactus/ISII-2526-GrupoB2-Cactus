
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

        public Review(int customerId, int customerCount, DateTime dateOfReview, int overallRating, bool reviewed, string reviewTitle, IList<ReviewItem> reviewItems)
        {
            CustomerId = customerId;
            CustomerCount = customerCount;
            DateOfReview = dateOfReview;
            OverallRating = overallRating;
            Reviewed = reviewed;
            ReviewTitle = reviewTitle;
            ReviewItems = reviewItems;
        }
        [Key]
        public int CustomerId { get; set; }

        [Range(1, 200, ErrorMessage = "Minimum 1, Maximum 200")]
        public int CustomerCount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Review")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfReview { get; set; }

        [Range(1, 200, ErrorMessage = "Minimum 1, Maximum 200")]
        public int OverallRating { get; set; }

        public bool Reviewed { get; set; }

        [StringLength(50, ErrorMessage = "Name can be neither longer than 50 characters nor shorter than 10.", MinimumLength = 10)]
        public string ReviewTitle { get; set; }

        public IList<ReviewItem> ReviewItems { get; set; }
    }

}
