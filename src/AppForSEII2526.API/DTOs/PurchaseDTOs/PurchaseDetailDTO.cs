using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseDetailDTO : PurchaseForCreateDTO
    {
        public PurchaseDetailDTO() { }

        public PurchaseDetailDTO(int id, string deliveryAddress, IList<PurchaseItemDTO> purchaseItems,
                                 string userName, string nameSurname, PaymentMethod paymentMethod,
                                 DateTime purchaseDate) :
            base(deliveryAddress, purchaseItems, userName, nameSurname, paymentMethod)
        {
            Id = id;
            PurchaseDate = purchaseDate;
        }

        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is PurchaseDetailDTO dTO &&
                   base.Equals(obj) &&
                   (PurchaseDate.Subtract(dTO.PurchaseDate) < new TimeSpan(0, 1, 0)) &&
                   Id == dTO.Id;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, PurchaseDate);
        }




    }
}