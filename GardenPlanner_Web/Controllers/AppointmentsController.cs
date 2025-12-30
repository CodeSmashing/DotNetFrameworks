using GardenPlanner_Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van afspraken in de webinterface.
	/// </summary>
	[Authorize(Roles = "User,UserAdmin,Admin")]
	public class AppointmentsController : Controller {
		private readonly AgendaDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="AppointmentsController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor afsprakenbeheer (dependency injection).
		/// </param>
		public AppointmentsController(AgendaDbContext context) {
			_context = context;
		}

		// GET: Appointments
		/// <summary>
		/// Toont een overzicht van alle beschikbare afspraken.
		/// </summary>
		/// <returns>
		/// De index view met een lijst van afspraken.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Index() {
			try {
				string? currentUserId = Request.HttpContext.GetUserId();
				if (string.IsNullOrEmpty(currentUserId)) {
					return View();
				}

				var agendaDbContext = _context.Appointments.Where(a => a.AgendaUserId == currentUserId && a.Deleted == null);
				ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-" && appt.Deleted == null), "Id", "Description");
				ViewData["UserId"] = currentUserId;
				ViewData["AppointmentTypeId"] = _context.AppointmentTypes.First(appt => appt.Id != "-").Id;
				return View(await agendaDbContext.ToListAsync());
			} catch (Exception ex) {
				// Log the exception (you can use a logging framework here)
				return View();
			}
		}

		// GET: Appointments/Details/5
		/// <summary>
		/// Toont de specifieke details van een enkele afspraak op basis van het ID.
		/// </summary>
		/// <param name="id">
		/// Het unieke ID van de afspraak.
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

			var appointment = await _context.Appointments
				.Include(a => a.AgendaUser)
				.Include(a => a.AppointmentType)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (appointment == null) {
				return NotFound();
			}

			return View(appointment);
		}

		// POST: Appointments/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de asynchrone aanmaak van een nieuwe afspraak via Unobtrusive Ajax.
		/// Deze methode ontvangt de formuliergegevens en retourneert een resultaat dat de
		/// UI gedeeltelijk bijwerkt.
		/// </summary>
		/// <param name="appointment">
		/// De gegevens van de nieuw aan te maken afspraak.</param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Create([Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment appointment) {
			ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", appointment.AppointmentType);

			if (ModelState.IsValid) {
				_context.Add(appointment);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(appointment);
		}

		// POST: Appointments/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt asynchroon de wijzigingen van een bestaande afspraak via Unobtrusive Ajax.
		/// Gebruikt om gegevens bij te werken zonder de gehele pagina te verversen.
		/// </summary>
		/// <param name="id">
		/// Het unieke ID van de te bewerken afspraak.
		/// </param>
		/// <param name="posted">
		/// De bijgewerkte gegevens die via Ajax zijn verzonden.
		/// </param>
		/// <returns>
		/// Een PartialView van de index als de gebruiker ID ongeldig is, 
		/// of een NotFound-resultaat als het meegegeven ID geen geldige afspraak kan terugvinden,
		/// of een PartialView van het bijgewerkte item bij succes of falen.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<PartialViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment posted) {
			string? currentUserId = Request.HttpContext.GetUserId();
			Appointment? targetAppointment = await _context.Appointments.FindAsync(id);

			if (string.IsNullOrEmpty(currentUserId)) {
				return PartialView(nameof(Index));
			}

			if (targetAppointment == null) {
				return NotFound();
			}

			var userAppointments = _context.Appointments.Where(a =>
				a.AgendaUserId == currentUserId
				&& a.Deleted == null);
			ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", posted.AppointmentType);

			if (ModelState.IsValid) {
				try {
					targetAppointment.Date = posted.Date;
					targetAppointment.Title = posted.Title;
					targetAppointment.Description = posted.Description;
					targetAppointment.AppointmentTypeId = posted.AppointmentTypeId;
					targetAppointment.IsApproved = posted.IsApproved;
					targetAppointment.IsCompleted = posted.IsCompleted;
					_context.Update(targetAppointment);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) when (!AppointmentExists(posted.Id)) {
					return NotFound();
				}
				return PartialView(nameof(Edit), targetAppointment);
			}
			return PartialView(nameof(Edit), targetAppointment);
		}

		// GET: Appointments/Delete/5
		/// <summary>
		/// Toont een bevestigingspagina voor het verwijderen van een specifieke afspraak.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te verwijderen afspraak.
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

			var appointment = await _context.Appointments
				.Include(a => a.AgendaUser)
				.Include(a => a.AppointmentType)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (appointment == null) {
				return NotFound();
			}

			return View(appointment);
		}

		// POST: Appointments/Delete/5
		/// <summary>
		/// Voert de daadwerkelijke verwijdering van de afspraak uit na bevestiging door de gebruiker.
		/// </summary>
		/// <param name="id">
		/// Het ID van de definitief te verwijderen afspraak.
		/// </param>
		/// <returns>
		/// Een redirect naar de index view na succesvolle verwijdering.
		/// </returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment != null) {
				//_context.Appointments.Remove(appointment);
				appointment.Deleted = DateTime.Now;
				_context.Update(appointment);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool AppointmentExists(string id) {
			return _context.Appointments.Any(e => e.Id == id);
		}
	}
}
