using System.Diagnostics.Tracing;
using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class ReviewStateContainer
    {
        public ReviewForCreateDTO Review { get; private set; } = new ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>() //Esto es como el carrito donde se guardan
        };

        public double AverageRating
        {
            get
            {
                return Review.ReviewItems.Any()
                    ? Review.ReviewItems.Average(ri => ri.Rating)
                    : 0;
            }
        }
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddDeviceToReview(DeviceForReviewDTO device)
        {
            if (!Review.ReviewItems.Any(ri => ri.Id == device.Id))
                Review.ReviewItems.Add(new ReviewItemDTO()
                {
                    Id = device.Id,
                    Name = device.Name,
                    Model = device.Model,
                    Year = device.Year,
                    Rating = 0,
                    Comments = null
                }
            );
            NotifyStateChanged();
        } //Agrega la reseña al carrito

        

        public void RemoveReviewItem(ReviewItemDTO item)
        {
            Review.ReviewItems.Remove(item);
            NotifyStateChanged();
        } //Elimina la reseña del carrito

        public void ClearReview()
        {
            Review.ReviewItems.Clear();
        }//Limpia el carrito

        public void ReviewProcessed()
        {
            Review = new ReviewForCreateDTO()
            {
                ReviewItems = new List<ReviewItemDTO>()
            };
        } //Cuando acaba vuelve a dejar el carrito vacio
    }
    }

