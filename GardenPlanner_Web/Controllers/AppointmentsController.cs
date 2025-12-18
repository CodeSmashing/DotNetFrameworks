using GardenPlanner_Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers {
	[Authorize(Roles = "User,UserAdmin,Admin")]
	public class AppointmentsController : Controller {
		private readonly AgendaDbContext _context;

		public AppointmentsController(AgendaDbContext context) {
			_context = context;
		}

		// GET: Appointments
		public async Task<IActionResult> Index() {
			try {
				string? currentUserId = Request.HttpContext.GetUserId();
				if (string.IsNullOrEmpty(currentUserId)) {
					return View();
				}

				var agendaDbContext = _context.Appointments.Where(a => a.AgendaUserId == currentUserId && a.Deleted == null);
				ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-" && appt.Deleted == null), "Id", "Description");
				ViewData["UserId"] = currentUserId;
				ViewData["AppointmentTypeId"] = _context.AppointmentTypes.First(appt => appt.Id != "-").Id;
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

		// POST: Appointments/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment appointment) {
			ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", appointment.AppointmentType);

			if (ModelState.IsValid) {
				_context.Add(appointment);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(appointment);
		}

		// POST: Appointments/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,AgendaUserId,Date,Title,Description,Created,Deleted,AppointmentTypeId,IsApproved,IsCompleted")] Appointment posted) {
			string? currentUserId = Request.HttpContext.GetUserId();
			Appointment? targetAppointment = await _context.Appointments.FindAsync(id);

			if (string.IsNullOrEmpty(currentUserId)) {
				return PartialView(nameof(Index));
			}

			if (targetAppointment == null) {
				return NotFound();
			}

			var userAppointments = _context.Appointments.Where(a =>
				a.AgendaUserId == currentUserId
				&& a.Deleted == null);
			ViewData["SelectListAppointmentType"] = new SelectList(_context.AppointmentTypes.Where(appt => appt.Id != "-"), "Id", "Name", posted.AppointmentType);

			if (ModelState.IsValid) {
				try {
					targetAppointment.Date = posted.Date;
					targetAppointment.Title = posted.Title;
					targetAppointment.Description = posted.Description;
					targetAppointment.AppointmentTypeId = posted.AppointmentTypeId;
					targetAppointment.IsApproved = posted.IsApproved;
					targetAppointment.IsCompleted = posted.IsCompleted;
					_context.Update(targetAppointment);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) {
					if (!AppointmentExists(posted.Id)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return PartialView(nameof(Edit), targetAppointment);
			}
			return PartialView(nameof(Edit), targetAppointment);
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
