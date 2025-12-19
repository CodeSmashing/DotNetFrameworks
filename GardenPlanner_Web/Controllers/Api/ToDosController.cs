using GardenPlanner_Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van to-do's.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class ToDosController : ControllerBase {
		private readonly AgendaDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="ToDosController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor to-do item beheer (dependency injection).
		/// </param>
		public ToDosController(AgendaDbContext context) {
			_context = context;
		}

		// GET: api/ToDos
		/// <summary>
		/// Haalt een lijst van alle to-do's op basis van afspraak ID op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die een lijst met
		/// <see cref="ToDo"/> objecten bevat.
		/// </returns>
		/// <response code="200">
		/// Retourneert de lijst met de to-do's.
		/// </response>
		[HttpGet("{appointmentId}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<IEnumerable<ToDo>>> GetToDos(string appointmentId) {
			return await _context.ToDos.Where(td => td.Deleted == null && td.AppointmentId == appointmentId).ToListAsync();
		}

		// GET: api/ToDos/5
		/// <summary>
		/// Haalt een specifieke to-do op basis van afspraak ID en to-do ID op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De unieke identificatie (GUID of string) van de to-do.
		/// </param>
		/// <param name="appointmentId">
		/// De unieke identificatie (GUID of string) van de afspraak.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> met het gevraagde
		/// <see cref="ToDo"/> object.
		/// </returns>
		/// <response code="200">
		/// Retourneert de gevraagde to-do.
		/// </response>
		/// <response code="404">
		/// Indien de to-do met de opgegeven ID niet gevonden is of als de
		/// afspraak ID overeenkomt.
		/// </response>
		[HttpGet("{appointmentId}/{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<ToDo>> GetToDo(string appointmentId, string id) {
			var toDo = await _context.ToDos.FindAsync(id);

			if (toDo == null || toDo.AppointmentId != appointmentId || toDo.Deleted == null) {
				return NotFound();
			}

			return toDo;
		}

		// PUT: api/ToDos/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Werkt een bestaande to-do bij.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de to-do die moet worden bijgewerkt.
		/// </param>
		/// <param name="toDo">
		/// De bijgewerkte to-do gegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de to-do succesvol is bijgewerkt (No Content).
		/// </response>
		/// <response code="400">
		/// Indien de opgegeven ID in de route niet overeenkomt met de
		/// ID in de body.
		/// </response>
		/// <response code="404">
		/// Indien de to-do niet gevonden is.
		/// </response>
		[HttpPut("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<IActionResult> PutToDo(string id, ToDo toDo) {
			if (id != toDo.Id) {
				return BadRequest();
			}

			_context.Entry(toDo).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!ToDoExists(id)) {
					return NotFound();
				} else {
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/ToDos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Maakt een nieuwe to-do aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="toDo">
		/// Het <see cref="ToDo"/> object dat moet worden toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// to-do retourneert.
		/// </returns>
		/// <response code="201">
		/// Retourneert het aangemaakte item (Created).
		/// </response>
		/// <response code="400">
		/// Indien de invoergegevens ongeldig zijn.
		/// </response>
		/// <response code="409">
		/// Als de to-do ID al bestaat in de database.
		/// </response>
		[HttpPost]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<ActionResult<ToDo>> PostToDo(ToDo toDo) {
			_context.ToDos.Add(toDo);
			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) {
				if (ToDoExists(toDo.Id)) {
					return Conflict();
				} else {
					throw;
				}
			}

			return CreatedAtAction("GetToDo", new {
				id = toDo.Id
			}, toDo);
		}

		// DELETE: api/ToDos/5
		/// <summary>
		/// Verwijdert een to-do op basis van ID. (Soft-delete)
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De ID van de to-do die moet worden verwijderd.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de to-do succesvol is verwijderd (No Content).
		/// </response>
		/// <response code="404">
		/// Indien de to-do niet gevonden is.
		/// </response>
		[HttpDelete("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		public async Task<IActionResult> DeleteToDo(string id) {
			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo == null) {
				return NotFound();
			}

			//_context.ToDos.Remove(toDo);
			toDo.Deleted = DateTime.Now;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ToDoExists(string id) {
			return _context.ToDos.Any(e => e.Id == id);
		}
	}
}
