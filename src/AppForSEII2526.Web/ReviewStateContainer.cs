using System.Diagnostics.Tracing;

namespace AppForSEII2526.Web
{
    public class ReviewStateContainer
    {
        public ReviewForCreateDTO Review { get; private set; } = new ReviewForCreateDTO()
        {
            ReviewItems = new List<ReviewItemDTO>()
        };

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
            ComputeAverageRating();
        }

        private void ComputeAverageRating()
        {
            Review.AverageRating = Review.ReviewItems.Any() ?
                (int)Review.ReviewItems.Average(ri => ri.Rating) : 0;
        }

        public void RemoveReviewItem(ReviewItemDTO item)
        {
            Review.ReviewItems.Remove(item);
            ComputeAverageRating();
        }

        public void ClearReview()
        {
            Review.ReviewItems.Clear();
            Review.AverageRating = 0;
        }

        public void ReviewProcessed()
        {
            Review = new ReviewForCreateDTO()
            {
                ReviewItems = new List<ReviewItemDTO>()
            };
        }
    }
    }

