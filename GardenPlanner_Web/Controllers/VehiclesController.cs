using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van voertuigen in de webinterface.
	/// </summary>
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class VehiclesController : Controller {
		private readonly GlobalDbContext _context;
		private readonly Utilities _utilities;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="VehiclesController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor voertuig beheer (dependency injection).
		/// </param>
		/// <param name="utilities">
		/// Hulp-methodes en voorzieningen (dependency injection).
		/// </param>
		public VehiclesController(
			GlobalDbContext context,
			Utilities utilities) {
			_context = context;
			_utilities = utilities;
		}

		// GET: Vehicles
		/// <summary>
		/// Toont een overzicht van alle beschikbare voertuigen.
		/// </summary>
		/// <returns>
		/// De index view met een lijst van voertuigen.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Index() {
			return View(await _context.Vehicles.Where(v => v.Id != "-" && v.Deleted == null).ToListAsync());
		}

		// GET: Vehicles/Details/5
		/// <summary>
		/// Toont de specifieke details van een enkele voertuig op basis van het ID.
		/// </summary>
		/// <param name="id">
		/// Het unieke ID van de voertuig.
		/// </param>
		/// <returns>
		/// De details view, of een NotFound-resultaat als het ID ontbreekt of ongeldig is.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Details(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles
				 .FirstOrDefaultAsync(m => m.Id == id);
			if (vehicle == null) {
				return NotFound();
			}

			return View(vehicle);
		}

		// GET: Vehicles/Create
		/// <summary>
		/// Toont het formulier om een nieuwe voertuig aan te maken.
		/// </summary>
		/// <returns>
		/// De create view.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public IActionResult Create() {
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>();
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>();
			return View();
		}

		// POST: Vehicles/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de invoer van het create-formulier en slaat de nieuwe voertuig op in de database.
		/// </summary>
		/// <param name="vehicle">
		/// Het object met de ingevoerde gegevens, gevalideerd via model binding.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Create([Bind("Id,Created,Deleted,LicencePlate,VehicleType,Brand,Model,LoadCapacity,WeightCapacity,FuelType,ImageUrl,IsManuel,IsInUse")] Vehicle vehicle) {
			if (!ModelState.IsValid) {
				ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>();
				ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>();
				return View(vehicle);
			}

			_context.Add(vehicle);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: Vehicles/Edit/5
		/// <summary>
		/// Toont het formulier om een bestaande voertuig te bewerken.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken voertuig.
		/// </param>
		/// <returns>
		/// De edit view met de huidige gegevens van het voertuig.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle == null) {
				return NotFound();
			}
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>(vehicle.VehicleType);
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>(vehicle.FuelType);
			return View(vehicle);
		}

		// POST: Vehicles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de wijzigingen van een bestaande voertuig en voert de update uit in de database.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken voertuig.
		/// </param>
		/// <param name="vehicle">
		/// De bijgewerkte gegevens van het voertuig.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Created,Deleted,LicencePlate,VehicleType,Brand,Model,LoadCapacity,WeightCapacity,FuelType,ImageUrl,IsManuel,IsInUse")] Vehicle vehicle) {
			if (id != vehicle.Id) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(vehicle);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) when (!VehicleExists(vehicle.Id)) {
					return NotFound();
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>(vehicle.VehicleType);
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>(vehicle.FuelType);
			return View(vehicle);
		}

		// GET: Vehicles/Delete/5
		/// <summary>
		/// Toont een bevestigingspagina voor het verwijderen van een specifieke voertuig.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te verwijderen voertuig.
		/// </param>
		/// <returns>
		/// De delete view ter bevestiging.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles
				.FirstOrDefaultAsync(m => m.Id == id);
			if (vehicle == null) {
				return NotFound();
			}

			return View(vehicle);
		}

		// POST: Vehicles/Delete/5
		/// <summary>
		/// Voert de daadwerkelijke verwijdering van het voertuig uit na bevestiging door de gebruiker.
		/// </summary>
		/// <param name="id">
		/// Het ID van de definitief te verwijderen voertuig.
		/// </param>
		/// <returns>
		/// Een redirect naar de index view na succesvolle verwijdering.
		/// </returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle != null) {
				//_context.Vehicles.Remove(vehicle);
				vehicle.Deleted = DateTime.Now;
				_context.Update(vehicle);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool VehicleExists(string id) {
			return _context.Vehicles.Any(e => e.Id == id);
		}
	}
}
