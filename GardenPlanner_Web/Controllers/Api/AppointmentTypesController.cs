using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Models.Extensions.Models;
using System.Net.Mime;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van afspraak types.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
	public class AppointmentTypesController : ControllerBase {
		private readonly GlobalDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="AppointmentTypesController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor afspraak types beheer (dependency injection).
		/// </param>
		public AppointmentTypesController(GlobalDbContext context) {
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
		/// <see cref="AppointmentTypeDTO"/> objecten bevat.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<IEnumerable<AppointmentTypeDTO>>(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<AppointmentTypeDTO>>> GetAppointmentTypes() {
			return Ok(await _context.AppointmentTypes
				.Where(appt => appt.Deleted == null)
				.Select(appt => appt.ToDTO())
				.ToListAsync());
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
		/// <see cref="AppointmentTypeDTO"/> object.
		/// </returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<AppointmentTypeDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<AppointmentTypeDTO>> GetAppointmentType(string id) {
			var appointmentType = await _context.AppointmentTypes.FindAsync(id);

			if (appointmentType == null || appointmentType.Deleted != null) {
				return NotFound();
			}

			return Ok(appointmentType.ToDTO());
		}

		// PUT: api/AppointmentTypes/5
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
		/// <param name="posted">
		/// De bijgewerkte afspraak type gegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpPut("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> PutAppointmentType(string id, AppointmentTypeDTO posted) {
			if (!string.IsNullOrEmpty(posted.GlobalId) || id != posted.GlobalId) {
				return BadRequest();
			}

			var appointmentType = await _context.AppointmentTypes.FindAsync(id);
			if (appointmentType == null) {
				return NotFound();
			}

			posted.ToExisting(appointmentType);
			_context.Entry(appointmentType).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) when (!AppointmentTypeExists(id)) {
				return NotFound();
			}

			return NoContent();
		}

		// POST: api/AppointmentTypes
		/// <summary>
		/// Maakt een nieuwe afspraak type aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="posted">
		/// Het <see cref="AppointmentTypeDTO"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// afspraak type retourneert.
		/// </returns>
		[HttpPost]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		[ProducesResponseType<AppointmentTypeDTO>(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<AppointmentTypeDTO>> PostAppointmentType(AppointmentTypeDTO posted) {
			AppointmentType appointmentType = posted.ToModel();
			if (appointmentType == null) {
				return BadRequest();
			}

			_context.AppointmentTypes.Add(appointmentType);

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) when (AppointmentTypeExists(appointmentType.Id)) {
				return Conflict();
			}

			return CreatedAtAction(
				nameof(GetAppointmentType),
				new {
					id = appointmentType.Id
				},
				appointmentType.ToDTO());
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
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeleteAppointmentType(string id) {
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
