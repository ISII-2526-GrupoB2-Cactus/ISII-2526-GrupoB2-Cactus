using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {

    [StringLength(30, ErrorMessage = "El nombre tiene que tener entre 3 y 30 caracteres",MinimumLength = 3)]
    public string CustomerUserName { get; set; }

    [StringLength(30, ErrorMessage = "El apellido tiene que tener entre 3 y 30 caracteres", MinimumLength = 3)]
    public string CustomerUserSurname { get; set; }

    [StringLength(30, ErrorMessage = "El nombre tiene que tener entre 3 y 30 caracteres", MinimumLength = 3)]
    public string? CustomerCountry { get; set; }

    public IList<Purchase> Purchase { get; set; }
    public IList<Review> Review { get; set; } 
    public IList<Rental> Rental { get; set; }   
}