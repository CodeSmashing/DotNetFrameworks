using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van afspraak types.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AppointmentTypesController : ControllerBase {
		private readonly AgendaDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="AppointmentTypesController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor afspraak types beheer (dependency injection).
		/// </param>
		public AppointmentTypesController(AgendaDbContext context) {
			_context = context;
		}

		// GET: api/AppointmentTypes
		/// <summary>
		/// Haalt een lijst van alle afspraken types op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die een lijst met
		/// <see cref="AppointmentType"/> objecten bevat.
		/// </returns>
		/// <response code="200">
		/// Retourneert de lijst met de afspraak types.
		/// </response>
		[HttpGet]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<IEnumerable<AppointmentType>>> GetAppointmentTypes() {
			return await _context.AppointmentTypes.Where(appt => appt.Deleted == null).ToListAsync();
		}

		// GET: api/AppointmentTypes/5
		/// <summary>
		/// Haalt een specifieke afspraak type op basis van ID op.
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
		/// <see cref="AppointmentType"/> object.
		/// </returns>
		/// <response code="200">
		/// Retourneert de gevraagde afspraak type.
		/// </response>
		/// <response code="404">
		/// Indien de afspraak type met de opgegeven ID niet gevonden is.
		/// </response>
		[HttpGet("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<AppointmentType>> GetAppointmentType(string id) {
			var appointmentType = await _context.AppointmentTypes.FindAsync(id);

			if (appointmentType == null) {
				return NotFound();
			}

			return appointmentType;
		}

		// PUT: api/AppointmentTypes/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Werkt een bestaande afspraak type bij.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de afspraak type die moet worden bijgewerkt.
		/// </param>
		/// <param name="appointmentType">
		/// De bijgewerkte afspraak type gegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de afspraak type succesvol is bijgewerkt (No Content).
		/// </response>
		/// <response code="400">
		/// Indien de opgegeven ID in de route niet overeenkomt met de
		/// ID in de body.
		/// </response>
		/// <response code="404">
		/// Indien de afspraak type niet gevonden is.
		/// </response>
		[HttpPut("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		public async Task<IActionResult> PutAppointmentType(string id, AppointmentType appointmentType) {
			if (id != appointmentType.Id) {
				return BadRequest();
			}

			_context.Entry(appointmentType).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!AppointmentTypeExists(id)) {
					return NotFound();
				} else {
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/AppointmentTypes
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Maakt een nieuwe afspraak type aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="appointmentType">
		/// Het <see cref="AppointmentType"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// afspraak type retourneert.
		/// </returns>
		/// <response code="201">
		/// Retourneert het aangemaakte item (Created).
		/// </response>
		/// <response code="400">
		/// Indien de invoergegevens ongeldig zijn.
		/// </response>
		/// <response code="409">
		/// Als de afspraak type ID al bestaat in de database.
		/// </response>
		[HttpPost]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		public async Task<ActionResult<AppointmentType>> PostAppointmentType(AppointmentType appointmentType) {
			_context.AppointmentTypes.Add(appointmentType);
			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) {
				if (AppointmentTypeExists(appointmentType.Id)) {
					return Conflict();
				} else {
					throw;
				}
			}

			return CreatedAtAction("GetAppointmentType", new {
				id = appointmentType.Id
			}, appointmentType);
		}

		// DELETE: api/AppointmentTypes/5
		/// <summary>
		/// Verwijdert een afspraak type op basis van ID. (Soft-delete)
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de afspraak type die moet worden verwijderd.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de afspraak type succesvol is verwijderd (No Content).
		/// </response>
		/// <response code="404">
		/// Indien de afspraak type niet gevonden is.
		/// </response>
		[HttpDelete("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		public async Task<IActionResult> DeleteAppointmentType(string id) {
			var appointmentType = await _context.AppointmentTypes.FindAsync(id);
			if (appointmentType == null) {
				return NotFound();
			}

			//_context.AppointmentTypes.Remove(appointmentType);
			appointmentType.Deleted = DateTime.Now;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool AppointmentTypeExists(string id) {
			return _context.AppointmentTypes.Any(e => e.Id == id);
		}
	}
}
