using AppForSEII2526.API.Models;
using AppForSEII2526.Web.API;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        // Creamos una instancia vacía de la compra
        public PurchaseForCreateDTO Purchase { get; private set; } = new PurchaseForCreateDTO()
        {
            PurchaseItems = new List<PurchaseItemDTO>()
        };

        // Cálculo automático del precio total (suma de items)
        public decimal TotalPrice
        {
            get
            {
                return Convert.ToDecimal(Purchase.PurchaseItems.Sum(pi => pi.PriceForPurchase * pi.Quantity));
            }
        }

        // Evento para notificar cambios a la UI
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();


        public void AddMovieToRental(DeviceForPurchaseDTO device)
        {
            //before adding a movie we checked whether it has been already added
            if (!Purchase.PurchaseItems.Any(pi => pi.DeviceID == device.Id))
                //we add it if it is not in the list
                Purchase.PurchaseItems.Add(new PurchaseItemDTO()
                {
                    DeviceID = device.Id,
                    Brand = device.Brand,
                    Model = device.Model,
                    Color = device.Color,
                    PriceForPurchase = device.PriceForPurchase,
                    Quantity = 1,
                    Description = null,


                }
            );

        }


        public void RemovePurchaseItem(PurchaseItemDTO item)
        {
            Purchase.PurchaseItems.Remove(item);
        }


        public void ClearPurchaseCart()
        {
            Purchase.PurchaseItems.Clear();
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