using AppForSEII2526.Web.API;


namespace AppForSEII2526.Web
{
    public class RentalStateContainer
    {

        //we create an instance of Rental when an instance of RentalStateContainer is created
        public RentalForCreateDTO Rental { get; private set; } = new RentalForCreateDTO()
        {
            RentalItems = new List<RentalItemDTO>()

        };


        //we compute the TotalPrice of the devices we have selected for renting them
        public decimal TotalPrice
        {
            get
            {
                int numberOfDays = (Rental.RentalDateTo - Rental.RentalDateFrom).Days;
                return Convert.ToDecimal(Rental.RentalItems.Sum(ri => ri.PriceForRent * ri.Quantity * numberOfDays));
            }
        }


        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();


        public void AddDeviceToRental(DeviceParaAlquilarDTO device)
        {
            //before adding a device we checked whether it has been already added
            if (!Rental.RentalItems.Any(ri => ri.DeviceId == device.Id))
                //we add it if it is not in the list
                Rental.RentalItems.Add(new RentalItemDTO()
                {
                    DeviceId = device.Id,
                    Name = device.Name,
                    Brand = device.Brand,
                    Model = device.Model,
                    PriceForRent = device.PriceForRent,
                    Quantity = 1 // Por defecto 1, pero podríamos permitir al usuario cambiar la cantidad?
                }
            );
        }


        //to delete movies from the list of selected movies
        public void RemoveRentalItemToRent(RentalItemDTO item)
        {
            Rental.RentalItems.Remove(item);

        }


        //we eliminate all the movies from the list
        public void ClearRentingCart()
        {
            Rental.RentalItems.Clear();

        }


        //we have already finished the process of renting, thus, we create a new Rental 
        public void RentalProcessed()
        {
            //we have finished the rental process so we create a new object without data
            Rental = new RentalForCreateDTO()
            {
                RentalItems = new List<RentalItemDTO>()
            };
        }



    }
}
