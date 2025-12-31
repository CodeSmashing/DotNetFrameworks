using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van afspraak types in de webinterface.
	/// </summary>
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class AppointmentTypesController : Controller {
		private readonly GlobalDbContext _context;
		private readonly Utilities _utilities;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="AppointmentTypesController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor afspraak types beheer (dependency injection).
		/// </param>
		/// <param name="utilities">
		/// Hulp-methodes en voorzieningen (dependency injection).
		/// </param>
		public AppointmentTypesController(
			GlobalDbContext context,
			Utilities utilities) {
			_context = context;
			_utilities = utilities;
		}

		// GET: AppointmentTypes
		/// <summary>
		/// Toont een overzicht van alle beschikbare afspraak types.
		/// </summary>
		/// <returns>
		/// De index view met een lijst van afspraak types.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Index() {
			return View(await _context.AppointmentTypes.Where(appt => appt.Id != "-" && appt.Deleted == null).ToListAsync());
		}

		// GET: AppointmentTypes/Details/5
		/// <summary>
		/// Toont de specifieke details van een enkele afspraak type op basis van het ID.
		/// </summary>
		/// <param name="id">
		/// Het unieke ID van de afspraak type.
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

			var appointmentType = await _context.AppointmentTypes
				 .FirstOrDefaultAsync(m => m.Id == id);
			if (appointmentType == null) {
				return NotFound();
			}

			return View(appointmentType);
		}

		// GET: AppointmentTypes/Create
		/// <summary>
		/// Toont het formulier om een nieuwe afspraak type aan te maken.
		/// </summary>
		/// <returns>
		/// De create view.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public IActionResult Create() {
			ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>();
			return View();
		}

		// POST: AppointmentTypes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de invoer van het create-formulier en slaat de nieuwe afspraak type
		/// op in de database.
		/// </summary>
		/// <param name="appointmentType">
		/// Het object met de ingevoerde gegevens, gevalideerd via model binding.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,Color,Created,Deleted")] AppointmentType appointmentType) {
			if (!ModelState.IsValid) {
				ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>();
				return View(appointmentType);
			}

			_context.Add(appointmentType);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: AppointmentTypes/Edit/5
		/// <summary>
		/// Toont het formulier om een bestaande afspraak type te bewerken.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken afspraak type.
		/// </param>
		/// <returns>
		/// De edit view met de huidige gegevens van de afspraak type.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id) {
			if (id == null) {
				return NotFound();
			}

			var appointmentType = await _context.AppointmentTypes.FindAsync(id);
			if (appointmentType == null) {
				return NotFound();
			}
			ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>(appointmentType.Name);
			return View(appointmentType);
		}

		// POST: AppointmentTypes/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de wijzigingen van een bestaande afspraak type en voert de update uit in de database.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken afspraak type.
		/// </param>
		/// <param name="appointmentType">
		/// De bijgewerkte gegevens van de afspraak type.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Color,Created,Deleted")] AppointmentType appointmentType) {
			if (id != appointmentType.Id) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(appointmentType);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) when (!AppointmentTypeExists(appointmentType.Id)) {
					return NotFound();
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>(appointmentType.Name);
			return View(appointmentType);
		}

		// GET: AppointmentTypes/Delete/5
		/// <summary>
		/// Toont een bevestigingspagina voor het verwijderen van een specifieke afspraak type.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te verwijderen afspraak type.
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

			var appointmentType = await _context.AppointmentTypes
				.FirstOrDefaultAsync(m => m.Id == id);
			if (appointmentType == null) {
				return NotFound();
			}

			return View(appointmentType);
		}

		// POST: AppointmentTypes/Delete/5
		/// <summary>
		/// Voert de daadwerkelijke verwijdering van het afspraak type uit na bevestiging
		/// door de gebruiker.
		/// </summary>
		/// <param name="id">
		/// Het ID van de definitief te verwijderen afspraak type.
		/// </param>
		/// <returns>
		/// Een redirect naar de index view na succesvolle verwijdering.
		/// </returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var appointmentType = await _context.AppointmentTypes.FindAsync(id);
			if (appointmentType != null) {
				//_context.AppointmentTypes.Remove(appointmentType);
				appointmentType.Deleted = DateTime.Now;
				_context.Update(appointmentType);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool AppointmentTypeExists(string id) {
			return _context.AppointmentTypes.Any(e => e.Id == id);
		}
	}
}
