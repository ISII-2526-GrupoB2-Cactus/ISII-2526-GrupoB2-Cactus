namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalDetailDTO : RentalForCreateDTO
    {


        public RentalDetailDTO(int id, DateTime rentalDate, string customerUserName, string customerNameSurname,
            string deliveryAddress, PaymentMethodType paymentMethod, DateTime rentalDateFrom,
            DateTime rentalDateTo, IList<RentalItemDTO> rentalItems)
            : base(customerUserName,
                   customerNameSurname,
                   deliveryAddress,
                   paymentMethod,
                   rentalDateFrom,
                   rentalDateTo,
                   rentalItems) 
        {
            Id = id;
            RentalDate = rentalDate;
        }

        

        public int Id { get; set; }

        public DateTime RentalDate { get; set; }


        public override bool Equals(object obj)
        {
            return obj is RentalDetailDTO dTO &&
                   base.Equals(obj) &&
                   Id == dTO.Id &&
                   CompareDate(RentalDate, dTO.RentalDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, RentalDate);
        }

        /*
        public override bool Equals(object? obj)
        {
            return obj is RentalDetailDTO dTO &&
                   base.Equals(obj) &&
                   TotalPrice == dTO.TotalPrice &&
                   Id == dTO.Id &&
                   CompareDate(RentalDate, dTO.RentalDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, RentalDate);
        }
        */
    }
}
