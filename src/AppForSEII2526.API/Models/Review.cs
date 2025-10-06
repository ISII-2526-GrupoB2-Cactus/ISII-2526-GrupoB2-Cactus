
namespace AppForSEII2526.API.Models
{
    public class Review
    {


        public Review()
        {
            //Constructor vacio
        }

        public Review(int customerId, int customerCount, DateTime dateOfReview, int overallRating, bool reviewed, string reviewTitle, ReviewItem reviewItem)
        {
            CustomerId = customerId;
            CustomerCount = customerCount;
            DateOfReview = dateOfReview;
            OverallRating = overallRating;
            Reviewed = reviewed;
            ReviewTitle = reviewTitle;
            ReviewItem = reviewItem;
        }

        public int CustomerId { get; set; }

        [Range(1, 200, ErrorMessage = "Minimum 1, Maximum 200")]
        public int CustomerCount { get; set; }

        public DateTime DateOfReview { get; set; }

        [Range(1, 200, ErrorMessage = "Minimum 1, Maximum 200")]
        public int OverallRating { get; set; }

        public bool Reviewed { get; set; }

        [StringLength(50, ErrorMessage = "Name can be neither longer than 50 characters nor shorter than 10.", MinimumLength = 10)]
        public string ReviewTitle { get; set; }

        public ReviewItem ReviewItem { get; set; } //Atributo para la relacion

    }
}
