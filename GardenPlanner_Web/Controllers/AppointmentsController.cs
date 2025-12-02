using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GardenPlanner_Web.Controllers {
	[Authorize(Roles = "User,UserAdmin,Admin")]
	public class AppointmentsController : Controller {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;

		public AppointmentsController(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
		}

		private Task<AgendaUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

		// GET: Appointments
		public async Task<IActionResult> Index() {
			try {
				AgendaUser? contextUser = GetCurrentUserAsync().Result;
				if (contextUser == null) {
					return View();
				}

				var agendaDbContext = _context.Appointments
					.Where(a => a.AgendaUserId == contextUser.Id && a.Deleted == null);
				return View(await agendaDbContext.ToListAsync());
			} catch (Exception ex) {
				// Log the exception (you can use a logging framework here)
				return View();
			}
		}

		// GET: Appointments/Details/5
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

		// GET: Appointments/Create
		public IActionResult Create() {
			AgendaUser? contextUser = GetCurrentUserAsync().Result;
			ViewData["AppointmentTypeId"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name");

			if (contextUser == null) {
				return View();
			}

			Appointment appointment = new() {
				AgendaUserId = contextUser.Id,
				Date = DateTime.Now,
				AppointmentTypeId = _context.AppointmentTypes.First(appt => appt.Id != "-").Id,
				Description = string.Empty,
				Title = string.Empty
			};
			return View(appointment);
		}

		// POST: Appointments/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment appointment) {
			if (ModelState.IsValid) {
				_context.Add(appointment);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["AppointmentTypeId"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", appointment.AppointmentType);
			return View(appointment);
		}

		// GET: Appointments/Edit/5
		public async Task<IActionResult> Edit(string id) {
			if (id == null) {
				return NotFound();
			}

			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) {
				return NotFound();
			}
			ViewData["AppointmentTypeId"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", appointment.AppointmentType);
			return View(appointment);
		}

		// POST: Appointments/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment appointment) {
			if (id != appointment.Id) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(appointment);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) {
					if (!AppointmentExists(appointment.Id)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["AppointmentTypeId"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", appointment.AppointmentType);
			return View(appointment);
		}

		// GET: Appointments/Delete/5
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
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
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
