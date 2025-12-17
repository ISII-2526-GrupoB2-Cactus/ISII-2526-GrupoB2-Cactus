using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Clave compuesta para RentDevice
        builder.Entity<RentDevice>().HasKey(rd => new { rd.RentId, rd.DeviceId });

        //Clave compuesta para PurchaseItem
        builder.Entity<PurchaseItem>().HasKey(pi => new { pi.PurchaseId, pi.DeviceId });

        //Clave compuesta para ReviewItem 
        builder.Entity<ReviewItem>().HasKey(ri => new { ri.ReviewId, ri.DeviceId });

        builder.Entity<Model>().HasAlternateKey(m => m.Name);
    }


    public DbSet<PurchaseItem> PurchaseItem { get; set; }
    public DbSet<RentDevice> RentDevice { get; set; }
    public DbSet<Rental> Rental { get; set; }
    public DbSet<Device> Device { get; set; }
    public DbSet<Model> Model { get; set; }
    public DbSet<Purchase> Purchase { get; set; }
    public DbSet<Review> Review { get; set; }   
    public DbSet<ReviewItem> ReviewItem { get; set; }

    
}
