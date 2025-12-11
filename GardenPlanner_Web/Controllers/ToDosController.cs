using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers {
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class ToDosController : Controller {
		private readonly AgendaDbContext _context;

		public ToDosController(AgendaDbContext context) {
			_context = context;
		}


		// GET: ToDos/Appointment/Id
		[HttpGet]
		[Route("ToDos/Appointment/{id}")]
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
		[HttpGet]
		[Route("ToDos/Appointment/Create/{id}")]
		public async Task<IActionResult> Create(string id) {
			var contextAppointment = await _context.Appointments.FindAsync(id);

			if (contextAppointment == null) {
				return RedirectToAction(nameof(Index));
			}
			ViewData["Appointment"] = contextAppointment;

			return View();
		}

		// POST: ToDos/Appointment/Create/id
		[HttpPost]
		[Route("ToDos/Appointment/Create/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string id, [Bind("Id,Created,Deleted,Description,Ready")] ToDo toDo) {
			// Ensure AppointmentId is set
			var contextAppointment = await _context.Appointments.FindAsync(id);

			if (contextAppointment == null) {
				return RedirectToAction(nameof(Index));
			}

			toDo.AppointmentId = id;
			ModelState.Remove(nameof(toDo.AppointmentId));
			ViewData["Appointment"] = contextAppointment;

			if (ModelState.IsValid) {
				_context.Add(toDo);
				await _context.SaveChangesAsync();
				return RedirectPermanent($"/ToDos/Appointment/{toDo.AppointmentId}");
			}

			return View();
		}

		// GET: ToDos/Edit/5
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
		[HttpPost]
		[ValidateAntiForgeryToken]
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
				} catch (DbUpdateConcurrencyException) {
					if (!ToDoExists(toDo.Id)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return RedirectPermanent($"/ToDos/Appointment/{toDo.AppointmentId}");
			}
			return View(toDo);
		}

		// GET: ToDos/Delete/5
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
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var toDo = await _context.ToDos.FindAsync(id);
			if (toDo != null) {
				//_context.ToDos.Remove(toDo);
				toDo.Deleted = DateTime.Now;
				_context.Update(toDo);
			}

			await _context.SaveChangesAsync();
			return RedirectPermanent($"/ToDos/Appointment/{toDo.AppointmentId}");
		}

		private bool ToDoExists(string id) {
			return _context.ToDos.Any(e => e.Id == id);
		}
	}
}
