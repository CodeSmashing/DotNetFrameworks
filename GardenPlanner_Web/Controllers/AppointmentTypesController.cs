using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace GardenPlanner_Web.Controllers {
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class AppointmentTypesController : Controller {
		private readonly AgendaDbContext _context;
		private readonly Utilities _utilities;

		public AppointmentTypesController(
			AgendaDbContext context,
			Utilities utilities) {
			_context = context;
			_utilities = utilities;
		}

		// GET: AppointmentTypes
		public async Task<IActionResult> Index() {
			return View(await _context.AppointmentTypes.Where(appt => appt.Id != "-" && appt.Deleted == null).ToListAsync());
		}

		// GET: AppointmentTypes/Details/5
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
		public IActionResult Create() {
			ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>();
			return View();
		}

		// POST: AppointmentTypes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Color,Created,Deleted")] AppointmentType appointmentType) {
			if (id != appointmentType.Id) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(appointmentType);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) {
					if (!AppointmentTypeExists(appointmentType.Id)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["NameItems"] = _utilities.GetEnumSelectList<AppointmentTypeName>(appointmentType.Name);
			return View(appointmentType);
		}

		// GET: AppointmentTypes/Delete/5
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
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
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
