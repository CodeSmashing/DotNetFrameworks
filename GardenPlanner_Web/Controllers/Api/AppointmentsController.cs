using GardenPlanner_Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.CustomServices;
using Models.DTO;
using System.Net.Mime;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het plannen en beheren
	/// van afspraken.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
	public class AppointmentsController : ControllerBase {
		private readonly GlobalDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="AppointmentsController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor afsprakenbeheer (dependency injection).
		/// </param>
		public AppointmentsController(GlobalDbContext context) {
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
		/// <see cref="AppointmentDTO"/> objecten bevat.
		/// </returns>
		[HttpGet]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<IEnumerable<AppointmentDTO>>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments() {
			string? userId = Request.HttpContext.GetUserId();

			if (userId == null) {
				return BadRequest(new {
					Message = "No to-do was found."
				});
			}

			return Ok(await _context.Appointments
				.Where(app => app.Deleted == null && app.AgendaUserId == userId)
				.Select(app => app.ToDTO())
				.ToListAsync());
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
		/// <see cref="AppointmentDTO"/> object.
		/// </returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<AppointmentDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<AppointmentDTO>> GetAppointment(string id) {
			var appointment = await _context.Appointments.FindAsync(id);

			if (appointment == null || appointment.Deleted != null) {
				return NotFound();
			}

			return Ok(appointment.ToDTO());
		}

		// PUT: api/Appointments/5
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
		/// <param name="appointmentDTO">
		/// De bijgewerkte afspraakgegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpPut("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> PutAppointment(string id, AppointmentDTO appointmentDTO) {
			if (id != appointmentDTO.Id) {
				return BadRequest();
			}

			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) {
				return NotFound();
			}

			appointment.Date = appointmentDTO.Date;
			appointment.Title = appointmentDTO.Title;
			appointment.Description = appointmentDTO.Description;
			appointment.AppointmentTypeId = appointmentDTO.AppointmentTypeId;
			_context.Entry(appointment).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) when (!AppointmentExists(id)) {
				return NotFound();
			}

			return NoContent();
		}

		// POST: api/Appointments
		/// <summary>
		/// Maakt een nieuwe afspraak aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="appointmentDTO">
		/// Het <see cref="AppointmentDTO"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// afspraak retourneert.
		/// </returns>
		[HttpPost]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<AppointmentDTO>(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<AppointmentDTO>> PostAppointment(AppointmentDTO appointmentDTO) {
			Appointment appointment = new() {
				AgendaUserId = appointmentDTO.AgendaUserId,
				Date = appointmentDTO.Date,
				Title = appointmentDTO.Title,
				Description = appointmentDTO.Description,
				AppointmentTypeId = appointmentDTO.AppointmentTypeId
			};

			_context.Appointments.Add(appointment);

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) when (AppointmentExists(appointmentDTO.Id)) {
				return Conflict();
			}

			return CreatedAtAction(
				nameof(PostAppointment),
				new {
					id = appointment.Id
				},
				appointment.ToDTO());
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
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
