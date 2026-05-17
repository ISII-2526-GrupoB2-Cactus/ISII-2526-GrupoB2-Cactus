using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
        {

            try
            {
                SeedUsers(serviceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar usuarios.");
            }

            try
            {
                SeedModels(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar modelos.");
            }

            try
            {
                SeedDevices(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar dispositivos.");
            }

            try
            {
                SeedPurchases(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar compras.");
            }
            /*
            try
            {
                SeedReviews(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar reseñas.");
            }


            try
            {
                SeedRentals(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al insertar alquileres.");
            }*/
        }

        // ----------------------------------------------------------
        // USERS
        // ----------------------------------------------------------
        private static void SeedUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (userManager.FindByEmailAsync("laura@alu.uclm.es").Result == null)
            {
                var user = new ApplicationUser
                {
                    CustomerUserName = "Laura",
                    CustomerUserSurname = "Gonzalez Rico",
                    UserName = "laura@alu.uclm.es",
                    Email = "laura@alu.uclm.es",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "Password123!").Wait();
            }

            if (userManager.FindByEmailAsync("rodrigo@alu.uclm.es").Result == null)
            {
                var user = new ApplicationUser
                {
                    CustomerUserName = "Elena",
                    CustomerUserSurname = "Organero Maroto",
                    UserName = "elena@alu.uclm.es",
                    Email = "elena@alu.uclm.es",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "Password123!").Wait();
            }

            if (userManager.FindByEmailAsync("Maria@alu.uclm.es").Result == null)
            {
                var user = new ApplicationUser
                {
                    CustomerUserName = "Maria",
                    CustomerUserSurname = "Martinez Gonzalez",
                    UserName = "maria@alu.uclm.es",
                    Email = "maria@alu.uclm.es",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "Password123!").Wait();
            }
        }

        // ----------------------------------------------------------
        // MODELS
        // ----------------------------------------------------------
        private static void SeedModels(ApplicationDbContext db)
        {
            if (db.Model.Any()) return;

            db.Model.AddRange(
                new Model { Name = "iPhone 15" },
                new Model { Name = "Galaxy S23" },
                new Model { Name = "PlayStation 5" },
                new Model { Name = "Surface Pro 9" },
                new Model { Name = "MacBook Air" }
            );

            db.SaveChanges();
        }

        // ----------------------------------------------------------
        // DEVICES
        // ----------------------------------------------------------
        private static void SeedDevices(ApplicationDbContext db)
        {
            if (db.Device.Any()) return;

            var models = db.Model.ToList();

            db.Device.AddRange(
                new Device { Brand = "Apple", Name = "iPhone 15", Color = "Negro", PriceForPurchase = 1200, QuantityForPurchase = 10, Year = 2023, Model = models[0] },
                new Device { Brand = "Samsung", Name = "Galaxy S23", Color = "Gris", PriceForPurchase = 999, QuantityForPurchase = 8, Year = 2023, Model = models[1] },
                new Device { Brand = "Sony", Name = "PlayStation 5", Color = "Blanco", PriceForPurchase = 550, QuantityForPurchase = 12, Year = 2023, Model = models[2] },
                new Device { Brand = "Microsoft", Name = "Surface Pro 9", Color = "Azul", PriceForPurchase = 1500, QuantityForPurchase = 7, Year = 2022, Model = models[3] },
                new Device { Brand = "Apple", Name = "MacBook Air", Color = "Plateado", PriceForPurchase = 1400, QuantityForPurchase = 5, Year = 2022, Model = models[4] }
            );

            db.SaveChanges();
        }

        // ----------------------------------------------------------
        // PURCHASES
        // ----------------------------------------------------------
        private static void SeedPurchases(ApplicationDbContext db)
        {
            if (db.Purchase.Any()) return;

            var users = db.Users.Take(3).ToList();
            var devices = db.Device.ToList();

            if (!users.Any() || !devices.Any()) return;

            var purchases = new List<Purchase>
            {
                new Purchase
                {
                    ApplicationUser = users[0],
                    DeliveryAddress = "Avda España 33, Albacete",
                    PaymentMethod = PaymentMethod.CreditCard,
                    PurchaseDate = DateTime.Now,
                    TotalPrice = devices[0].PriceForPurchase,
                    TotalQuantity = 1,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { DeviceId = devices[0].Id, Quantity = 1, Price = devices[0].PriceForPurchase }
                    }
                },

                new Purchase
                {
                    ApplicationUser = users[1],
                    DeliveryAddress = "Calle Mayor 12, Toledo",
                    PaymentMethod = PaymentMethod.PayPal,
                    PurchaseDate = DateTime.Now.AddMinutes(-20),
                    TotalPrice = devices[1].PriceForPurchase * 2,
                    TotalQuantity = 2,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { DeviceId = devices[1].Id, Quantity = 2, Price = devices[1].PriceForPurchase }
                    }
                },

                new Purchase
                {
                    ApplicationUser = users[2],
                    DeliveryAddress = "Calle Libertad 9, Ciudad Real",
                    PaymentMethod = PaymentMethod.CreditCard,
                    PurchaseDate = DateTime.Now.AddHours(-1),
                    TotalPrice = devices[2].PriceForPurchase,
                    TotalQuantity = 1,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { DeviceId = devices[2].Id, Quantity = 1, Price = devices[2].PriceForPurchase }
                    }
                },

                new Purchase
                {
                    ApplicationUser = users[0],
                    DeliveryAddress = "Avda de la Mancha 77, Albacete",
                    PaymentMethod = PaymentMethod.CreditCard,
                    PurchaseDate = DateTime.Now.AddDays(-1),
                    TotalPrice = devices[3].PriceForPurchase + devices[4].PriceForPurchase,
                    TotalQuantity = 2,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { DeviceId = devices[3].Id, Quantity = 1, Price = devices[3].PriceForPurchase },
                        new PurchaseItem { DeviceId = devices[4].Id, Quantity = 1, Price = devices[4].PriceForPurchase }
                    }
                },

                new Purchase
                {
                    ApplicationUser = users[1],
                    DeliveryAddress = "Calle Valencia 30, Cuenca",
                    PaymentMethod = PaymentMethod.PayPal,
                    PurchaseDate = DateTime.Now.AddDays(-2),
                    TotalPrice = devices[0].PriceForPurchase * 3,
                    TotalQuantity = 3,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { DeviceId = devices[0].Id, Quantity = 3, Price = devices[0].PriceForPurchase }
                    }
                }
            };

            db.Purchase.AddRange(purchases);
            db.SaveChanges();
        }


        // ----------------------------------------------------------
        // REVIEWS
        // ----------------------------------------------------------
        /*
        private static void SeedReviews(ApplicationDbContext db)
        {
            if (db.Review.Any()) return;

            var users = db.Users.Take(3).ToList();

            if (!users.Any()) return;

            var reviews = new List<Review>
    {
        new Review
        {
            CustomerId = users[0].Id,
            DateOfReview = DateTime.Now.AddDays(-10),
            OverallRating = 5,
            ReviewTitle = "Excelente experiencia de compra",
            ApplicationUser = users[0]
        },
        new Review
        {
            CustomerId = users[1].Id,
            DateOfReview = DateTime.Now.AddDays(-8),
            OverallRating = 4,
            ReviewTitle = "Muy buena atención al cliente",
            ApplicationUser = users[1]
        },
        new Review
        {
            CustomerId = users[2].Id,
            DateOfReview = DateTime.Now.AddDays(-15),
            OverallRating = 5,
            ReviewTitle = "Servicio rápido y eficiente",
            ApplicationUser = users[2]
        },
        new Review
        {
            CustomerId = users[0].Id,
            DateOfReview = DateTime.Now.AddDays(-12),
            OverallRating = 3,
            ReviewTitle = "Producto bueno con entrega regular",
            ApplicationUser = users[0]
        },
        new Review
        {
            CustomerId = users[1].Id,
            DateOfReview = DateTime.Now.AddDays(-5),
            OverallRating = 5,
            ReviewTitle = "Calidad premium garantizada",
            ApplicationUser = users[1]
        },
        new Review
        {
            CustomerId = users[2].Id,
            DateOfReview = DateTime.Now.AddDays(-3),
            OverallRating = 4,
            ReviewTitle = "Buen precio y buena calidad",
            ApplicationUser = users[2]
        },
        new Review
        {
            CustomerId = users[0].Id,
            DateOfReview = DateTime.Now.AddDays(-7),
            OverallRating = 5,
            ReviewTitle = "Recomendado totalmente",
            ApplicationUser = users[0]
        },
        new Review
        {
            CustomerId = users[1].Id,
            DateOfReview = DateTime.Now.AddDays(-2),
            OverallRating = 4,
            ReviewTitle = "Satisfecho con la compra",
            ApplicationUser = users[1]
        }
    };

            db.Review.AddRange(reviews);
            db.SaveChanges();
        }


        // ----------------------------------------------------------
        // RENTALS
        // ----------------------------------------------------------

        private static void SeedRentals(ApplicationDbContext db)
        {
            if (db.Rental.Any()) return;

            var users = db.Users.Take(3).ToList();

            if (!users.Any()) return;

            var rentals = new List<Rental>
    {
        new Rental
        {
            DeliveryAddress = "Avda España 33, Albacete",
            NameCustomer = "Laura",
            SurnameCustomer = "Gonzalez Rico",
            TotalPrice = 150.50m,
            RentalDate = DateTime.Now.AddDays(-5),
            RentalDateFrom = DateTime.Now.AddDays(-5),
            RentalDateTo = DateTime.Now.AddDays(2),
            PaymentMethod = PaymentMethodType.CreditCard,
            ApplicationUser = users[0]
        },
        new Rental
        {
            DeliveryAddress = "Calle Mayor 12, Toledo",
            NameCustomer = "Elena",
            SurnameCustomer = "Organero Maroto",
            TotalPrice = 89.99m,
            RentalDate = DateTime.Now.AddDays(-3),
            RentalDateFrom = DateTime.Now.AddDays(-3),
            RentalDateTo = DateTime.Now.AddDays(7),
            PaymentMethod = PaymentMethodType.PayPal,
            ApplicationUser = users[1]
        },
        new Rental
        {
            DeliveryAddress = "Calle Libertad 9, Ciudad Real",
            NameCustomer = "Maria",
            SurnameCustomer = "Martinez Gonzalez",
            TotalPrice = 220.75m,
            RentalDate = DateTime.Now.AddDays(-1),
            RentalDateFrom = DateTime.Now.AddDays(-1),
            RentalDateTo = DateTime.Now.AddDays(14),
            PaymentMethod = PaymentMethodType.CreditCard,
            ApplicationUser = users[2]
        },
        new Rental
        {
            DeliveryAddress = "Avda de la Mancha 77, Albacete",
            NameCustomer = "Laura",
            SurnameCustomer = "Gonzalez Rico",
            TotalPrice = 300.00m,
            RentalDate = DateTime.Now.AddDays(-7),
            RentalDateFrom = DateTime.Now.AddDays(-7),
            RentalDateTo = DateTime.Now.AddDays(0),
            PaymentMethod = PaymentMethodType.Cash,
            ApplicationUser = users[0]
        },
        new Rental
        {
            DeliveryAddress = "Calle Valencia 30, Cuenca",
            NameCustomer = "Elena",
            SurnameCustomer = "Organero Maroto",
            TotalPrice = 175.25m,
            RentalDate = DateTime.Now.AddDays(-2),
            RentalDateFrom = DateTime.Now.AddDays(-2),
            RentalDateTo = DateTime.Now.AddDays(5),
            PaymentMethod = PaymentMethodType.PayPal,
            ApplicationUser = users[1]
        }
    };

            db.Rental.AddRange(rentals);
            db.SaveChanges();
        }*/

    }
}