using AppForSEII2526.API.Models;
using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        public PurchaseForCreateDTO Purchase { get; private set; } = new PurchaseForCreateDTO()
        {
            PurchaseItems = new List<PurchaseItemDTO>()
        };

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddDeviceForPurchase(DeviceForPurchaseDTO device)
        {
            if (!Purchase.PurchaseItems.Any(pi => pi.DeviceID == device.Id))
            {
                Purchase.PurchaseItems.Add(new PurchaseItemDTO
                {
                    DeviceID = device.Id,
                    Brand = device.Brand,
                    Model = device.Model,
                    Color = device.Color,
                    PriceForPurchase = device.PriceForPurchase,
                    Quantity = 1,
                    Description = null
                });

                ComputeTotalPrice();
            }
        }

        private void ComputeTotalPrice()
        {
            Purchase.TotalPrice = Purchase.PurchaseItems.Sum(pi => pi.PriceForPurchase * pi.Quantity);
        }


        public void RemovePurchaseItem(PurchaseItemDTO item)
        {

            Purchase.PurchaseItems.Remove(item);
            ComputeTotalPrice();
        }

        public void ClearPurchaseCart()
        {
            Purchase.PurchaseItems.Clear();
            Purchase.TotalPrice = 0;
        }

        public void PurchaseProcessed()
        {
            Purchase = new PurchaseForCreateDTO()
            {
                PurchaseItems = new List<PurchaseItemDTO>()
            };
        }
    }
}