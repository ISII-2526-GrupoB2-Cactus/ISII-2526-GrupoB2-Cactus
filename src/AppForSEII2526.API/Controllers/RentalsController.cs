using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RentalsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<RentalsController> _logger;

		public RentalsController(ApplicationDbContext context, ILogger<RentalsController> logger)
		{
			_context = context;
			_logger = logger;
		}



		[HttpGet]
		[Route("[action]")]
		[ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<ActionResult> GetRental(int id)
		{
			if (_context.Rental == null)
			{
				_logger.LogError("Error: Rentals table does not exist");
				return NotFound();
			}

			var rental = await _context.Rental
			 .Where(r => r.Id == id)
				 .Include(r => r.RentDevices)
					.ThenInclude(rd => rd.Device)
						.ThenInclude(device => device.Model)
			 .Select(r => new RentalDetailDTO(r.Id, r.RentalDate, r.NameCustomer,
					r.SurnameCustomer, r.DeliveryAddress,
					(PaymentMethodType)r.PaymentMethod,
					r.RentalDateFrom, r.RentalDateTo,
					r.RentDevices
						.Select(rd => new RentalItemDTO(rd.Device.Id,
								rd.Device.Name, rd.Device.Brand,
								rd.Device.Model.Name, rd.Device.PriceForRent, rd.Quantity)).ToList<RentalItemDTO>()))
			 .FirstOrDefaultAsync();

			if (rental == null)
			{
				_logger.LogError($"Error: Rental with id {id} does not exist");
				return NotFound();
			}

			return Ok(rental);
		}




		[HttpPost]
		[Route("[action]")]
		[ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
		public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
		{
			//any validation defined in PurchaseForCreate is checked before running the method so they don't have to be checked again
			if (rentalForCreate.RentalDateFrom <= DateTime.Today)
				ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

			if (rentalForCreate.RentalDateFrom >= rentalForCreate.RentalDateTo)
				ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

			if (rentalForCreate.RentalItems.Count == 0)
				ModelState.AddModelError("RentalItems", "Error! You must include at least one device to be rented");

			//Esto se comenta si no queremos alquileres con usuario registrado
			var user = _context.Users.FirstOrDefault(au => au.UserName == rentalForCreate.CustomerUserName);
			if (user == null)
				ModelState.AddModelError("RentalApplicationUser", "Error! UserName is not registered");


            //MODIFICACION
            if (rentalForCreate.DeliveryAddress != null && !(rentalForCreate.DeliveryAddress.Contains("Calle") || rentalForCreate.DeliveryAddress.Contains("Carretera")) )
                ModelState.AddModelError("DeliveryAddress", "Error! Invalid delivery address. Please include 'Calle' or 'Carretera' in the address");




            if (ModelState.ErrorCount > 0)
				return BadRequest(new ValidationProblemDetails(ModelState));

			

			var deviceNames = rentalForCreate.RentalItems.Select(ri => ri.Name).ToList<string>();

			var devices = _context.Device.Include(d => d.RentedDevices)
				.ThenInclude(rd => rd.Rental)
				.Where(d => deviceNames.Contains(d.Name))  //FILTRAR POR NAME
				.Select(d => new
				{
					d.Id,
					d.Name,
					d.QuantityForRent,
					d.PriceForRent,
					NumberOfRentedItems = d.RentedDevices.Count(rd => rd.Rental.RentalDateFrom <= rentalForCreate.RentalDateTo
							&& rd.Rental.RentalDateTo >= rentalForCreate.RentalDateFrom)
				})
				.ToList();



			Rental rental = new Rental(
				rentalForCreate.CustomerUserName,
				rentalForCreate.CustomerNameSurname,
				user, //null   Se comenta si no quermemos alquileres de usuarios registrados                                   
				rentalForCreate.DeliveryAddress,
				DateTime.Now,
				(PaymentMethodType)rentalForCreate.PaymentMethod,
				rentalForCreate.RentalDateFrom,
				rentalForCreate.RentalDateTo,
				new List<RentDevice>()
			);



			rental.TotalPrice = 0;
			var numDays = (rental.RentalDateTo - rental.RentalDateFrom).TotalDays;



			foreach (var item in rentalForCreate.RentalItems)
			{
				var device = devices.FirstOrDefault(d => d.Name == item.Name);

				if ((device == null) || (device.NumberOfRentedItems >= device.QuantityForRent))
				{
					ModelState.AddModelError("RentalItems", $"Error! Device Name '{item.Name}' is not available for being rented from {rentalForCreate.RentalDateFrom.ToShortDateString()} to {rentalForCreate.RentalDateTo.ToShortDateString()}");
				}
				else
				{

					rental.RentDevices.Add(new RentDevice
					{
						Rental = rental,
						DeviceId = device.Id,
						Price = device.PriceForRent,
						Quantity = item.Quantity
					});

					item.PriceForRent = device.PriceForRent;
				}
			}
			rental.TotalPrice = (decimal)rental.RentDevices.Sum(rd => rd.Price * numDays * rd.Quantity);




			if (ModelState.ErrorCount > 0)
			{
				return BadRequest(new ValidationProblemDetails(ModelState));
			}

			_context.Add(rental);

			try
			{
				//we store in the database both rental and its rentalitems
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				_logger.LogError(ex.InnerException?.Message ?? "No inner exception");
				ModelState.AddModelError("Rental", $"Error: {ex.Message} - Inner: {ex.InnerException?.Message}");
				return BadRequest(new ValidationProblemDetails(ModelState));
			}


			var rentalDetail = new RentalDetailDTO(rental.Id, rental.RentalDate,
				rental.NameCustomer, rental.SurnameCustomer,
				rental.DeliveryAddress, rentalForCreate.PaymentMethod,
				rental.RentalDateFrom, rental.RentalDateTo,
				rentalForCreate.RentalItems);

			return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
		}
	}
}





