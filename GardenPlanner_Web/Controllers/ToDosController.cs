using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van to-do's in de webinterface.
	/// </summary>
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class ToDosController : Controller {
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

		// GET: ToDos/Appointment/Id
		/// <summary>
		/// Toont een overzicht van alle beschikbare to-do's van een afspraak.
		/// </summary>
		/// <returns>
		/// De index view met een lijst van to-do's van een afspraak.
		/// </returns>
		[HttpGet("todos/appointments/{id}")]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Index(string id) {
			var contextAppointment = await _context.Appointments.FindAsync(id);

			if (contextAppointment == null) {
				return RedirectToAction(nameof(Index));
			}
			ViewData["Appointment"] = contextAppointment;

			return View(await _context.ToDos
				.Include(t => t.Appointment)
				.Where(t =>
					t.AppointmentId == id
					&& t.Deleted == null)
				.ToListAsync());
		}

		// GET: ToDos/Details/5
		/// <summary>
		/// Toont de specifieke details van een enkele to-do op basis van het ID.
		/// </summary>
		/// <param name="id">
		/// Het unieke ID van de to-do.
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

			var toDo = await _context.ToDos
				.Include(t => t.Appointment)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (toDo == null) {
				return NotFound();
			}

			return View(toDo);
		}


		// GET: ToDos/Appointment/Create/Id
		/// <summary>
		/// Toont het formulier om een nieuwe to-do aan te maken.
		/// </summary>
		/// <returns>
		/// De create view.
		/// </returns>
		[HttpGet("todos/appointments/create/{id}")]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Create(string id) {
			var contextAppointment = await _context.Appointments.FindAsync(id);

			if (contextAppointment == null) {
				return RedirectToAction(nameof(Index));
			}
			ViewData["Appointment"] = contextAppointment;

			return View();
		}

		// POST: ToDos/Appointment/Create/id
		/// <summary>
		/// Verwerkt de invoer van het create-formulier en slaat de nieuwe to-do op in de database.
		/// </summary>
		/// <param name="id">
		/// De ID van de geassocieerde afspraak.
		/// </param>
		/// <param name="toDo">
		/// Het object met de ingevoerde gegevens, gevalideerd via model binding.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost("todos/appointments/create/{id}")]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> Create(string id, [Bind("Id,Created,Deleted,Description,Ready")] ToDo toDo) {
			// Ensure the appointment ID is valid
			var contextAppointment = await _context.Appointments.FindAsync(id);

			if (contextAppointment == null) {
				return RedirectToAction(nameof(Index));
			}

			toDo.AppointmentId = id;
			ModelState.Remove(nameof(toDo.AppointmentId));
			ViewData["Appointment"] = contextAppointment;

			if (!ModelState.IsValid) {
				return View(toDo);
			}

			_context.Add(toDo);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: ToDos/Edit/5
		/// <summary>
		/// Toont het formulier om een bestaande to-do te bewerken.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken to-do.
		/// </param>
		/// <returns>
		/// De edit view met de huidige gegevens van de to-do.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id) {
			if (id == null) {
				return NotFound();
			}

			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo == null) {
				return NotFound();
			}
			return View(toDo);
		}

		// POST: ToDos/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		/// <summary>
		/// Verwerkt de wijzigingen van een bestaande to-do en voert de update uit in de database.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te bewerken to-do.
		/// </param>
		/// <param name="posted">
		/// De bijgewerkte gegevens van de to-do.
		/// </param>
		/// <returns>
		/// Een redirect naar de index bij succes, of de view met foutmeldingen bij een ongeldige invoer.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Created,Deleted,Description,Ready,AppointmentId")] ToDo posted) {
			// Re-fetch the original ToDo as ASP.NET Core isn't made to re-use what was passed in the GET
			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo == null) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					toDo.Description = posted.Description;
					toDo.Ready = posted.Ready;
					_context.Update(toDo);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) when (!ToDoExists(toDo.Id)) {
					return NotFound();
				}
				return RedirectToAction(nameof(Index));
			}
			return View(toDo);
		}

		// GET: ToDos/Delete/5
		/// <summary>
		/// Toont een bevestigingspagina voor het verwijderen van een specifieke to-do.
		/// </summary>
		/// <param name="id">
		/// Het ID van de te verwijderen to-do.
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

			var toDo = await _context.ToDos
				.Include(t => t.Appointment)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (toDo == null) {
				return NotFound();
			}

			return View(toDo);
		}

		// POST: ToDos/Delete/5
		/// <summary>
		/// Voert de daadwerkelijke verwijdering van de to-do uit na bevestiging door de gebruiker.
		/// </summary>
		/// <param name="id">
		/// Het ID van de definitief te verwijderen to-do.
		/// </param>
		/// <returns>
		/// Een redirect naar de index view na succesvolle verwijdering.
		/// </returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo != null) {
				//_context.ToDos.Remove(toDo);
				toDo.Deleted = DateTime.Now;
				_context.Update(toDo);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ToDoExists(string id) {
			return _context.ToDos.Any(e => e.Id == id);
		}
	}
}
