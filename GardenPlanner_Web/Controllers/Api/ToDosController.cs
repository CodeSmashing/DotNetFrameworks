using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using Models.Extensions.Models;
using System.Net.Mime;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van to-do's.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
	public class ToDosController : ControllerBase {
		private readonly GlobalDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="ToDosController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor to-do item beheer (dependency injection).
		/// </param>
		public ToDosController(GlobalDbContext context) {
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
		/// <see cref="ToDoDTO"/> objecten bevat.
		/// </returns>
		[HttpGet("{appointmentId}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<IEnumerable<ToDoDTO>>(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<ToDoDTO>>> GetToDos(string appointmentId) {
			return Ok(await _context.ToDos
				.Where(td => td.Deleted == null && td.AppointmentId == appointmentId)
				.Select(td => td.ToDTO())
				.ToListAsync());
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
		/// <see cref="ToDoDTO"/> object.
		/// </returns>
		[HttpGet("{appointmentId}/{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<ToDoDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ToDoDTO>> GetToDo(string appointmentId, string id) {
			var toDo = await _context.ToDos.FindAsync(id);

			if (toDo == null || toDo.AppointmentId != appointmentId || toDo.Deleted == null) {
				return NotFound();
			}

			return Ok(toDo.ToDTO());
		}

		// PUT: api/ToDos/5
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
		/// <param name="toDoDTO">
		/// De bijgewerkte to-do gegevens in de body van het verzoek.
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
		public async Task<ActionResult> PutToDo(string id, ToDoDTO toDoDTO) {
			if (id != toDoDTO.Id) {
				return BadRequest();
			}

			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo == null) {
				return NotFound();
			}

			toDo.Description = toDoDTO.Description;
			toDo.AppointmentId = toDoDTO.AppointmentId;
			_context.Entry(toDo).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) when (!ToDoExists(id)) {
				return NotFound();
			}

			return NoContent();
		}

		// POST: api/ToDos
		/// <summary>
		/// Maakt een nieuwe to-do aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "User", "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="toDoDTO">
		/// Het <see cref="ToDoDTO"/> object dat moet worden toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// to-do retourneert.
		/// </returns>
		[HttpPost]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType<ToDoDTO>(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<ToDoDTO>> PostToDo(ToDoDTO toDoDTO) {
			ToDo toDo = new() {
				Description = toDoDTO.Description,
				AppointmentId = toDoDTO.AppointmentId
			};

			_context.ToDos.Add(toDo);

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) when (ToDoExists(toDo.Id)) {
				return Conflict();
			}

			return CreatedAtAction(
				nameof(PostToDo),
				new {
					id = toDo.Id
				},
				toDo.ToDTO());
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
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "User,UserAdmin,Admin,Employee")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeleteToDo(string id) {
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
