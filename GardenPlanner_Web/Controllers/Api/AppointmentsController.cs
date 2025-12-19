using GardenPlanner_Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het plannen en beheren
	/// van afspraken.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AppointmentsController : ControllerBase {
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

		// GET: api/Appointments
		/// <summary>
		/// Haalt een lijst van alle afspraken van de aangemelde
		/// gebruiker op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die een lijst met
		/// <see cref="Appointment"/> objecten bevat.
		/// </returns>
		/// <response code="200">
		/// Retourneert de lijst met afspraken.
		/// </response>
		[HttpGet]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments() {
			string? userId = Request.HttpContext.GetUserId();
			return await _context.Appointments.Where(app => app.Deleted == null && app.AgendaUserId == userId).ToListAsync();
		}

		// GET: api/Appointments/5
		/// <summary>
		/// Haalt een specifieke afspraak op basis van ID op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De unieke identificatie (GUID of string) van de afspraak.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> met het gevraagde
		/// <see cref="Appointment"/> object.
		/// </returns>
		/// <response code="200">
		/// Retourneert de gevraagde afspraak.
		/// </response>
		/// <response code="404">
		/// Indien de afspraak met de opgegeven ID niet gevonden is.
		/// </response>
		[HttpGet("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<Appointment>> GetAppointment(string id) {
			var appointment = await _context.Appointments.FindAsync(id);

			if (appointment == null || appointment.Deleted != null) {
				return NotFound();
			}

			return appointment;
		}

		// PUT: api/Appointments/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Werkt een bestaande afspraak bij.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de afspraak die moet worden bijgewerkt.
		/// </param>
		/// <param name="appointment">
		/// De bijgewerkte afspraakgegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de afspraak succesvol is bijgewerkt (No Content).
		/// </response>
		/// <response code="400">
		/// Indien de opgegeven ID in de route niet overeenkomt met de
		/// ID in de body.
		/// </response>
		/// <response code="404">
		/// Indien de afspraak niet gevonden is.
		/// </response>
		[HttpPut("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<IActionResult> PutAppointment(string id, Appointment appointment) {
			if (id != appointment.Id) {
				return BadRequest();
			}

			_context.Entry(appointment).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!AppointmentExists(id)) {
					return NotFound();
				} else {
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Appointments
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Maakt een nieuwe afspraak aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="appointment">
		/// Het <see cref="Appointment"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// afspraak retourneert.
		/// </returns>
		/// <response code="201">
		/// Retourneert het aangemaakte item (Created).
		/// </response>
		/// <response code="400">
		/// Indien de invoergegevens ongeldig zijn.
		/// </response>
		/// <response code="409">
		/// Als de afspraak ID al bestaat in de database.
		/// </response>
		[HttpPost]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment) {
			_context.Appointments.Add(appointment);
			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) {
				if (AppointmentExists(appointment.Id)) {
					return Conflict();
				} else {
					throw;
				}
			}

			return CreatedAtAction("GetAppointment", new {
				id = appointment.Id
			}, appointment);
		}

		// DELETE: api/Appointments/5
		/// <summary>
		/// Verwijdert een afspraak op basis van ID. (Soft-delete)
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de afspraak die moet worden verwijderd.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de afspraak succesvol is verwijderd (No Content).
		/// </response>
		/// <response code="404">
		/// Indien de afspraak niet gevonden is.
		/// </response>
		[HttpDelete("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<IActionResult> DeleteAppointment(string id) {
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) {
				return NotFound();
			}

			//_context.Appointments.Remove(appointment);
			appointment.Deleted = DateTime.Now;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool AppointmentExists(string id) {
			return _context.Appointments.Any(e => e.Id == id);
		}
	}
}
