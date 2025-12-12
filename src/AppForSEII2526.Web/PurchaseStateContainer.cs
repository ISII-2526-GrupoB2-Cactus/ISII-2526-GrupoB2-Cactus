using AppForSEII2526.API.DTOs.PurchaseDTOs;

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        public PurchaseForCreateDTO Purchase { get; private set; }
            = new PurchaseForCreateDTO();

        public decimal TotalPrice => Convert.ToDecimal(Purchase.PurchaseItems.Sum(pi => pi.PriceForPurchase * pi.Quantity));

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

                NotifyStateChanged();
            }
        }


        public void RemovePurchaseItem(PurchaseItemDTO item)
        {
            Purchase.PurchaseItems.Remove(item);
            NotifyStateChanged();
        }

        public void ClearPurchaseCart()
        {
            Purchase.PurchaseItems.Clear();
            NotifyStateChanged();
        }

        public void PurchaseProcessed()
        {
            Purchase = new PurchaseForCreateDTO();
            NotifyStateChanged();
        }
    }
}
