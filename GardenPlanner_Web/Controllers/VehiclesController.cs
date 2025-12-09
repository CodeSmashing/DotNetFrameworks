using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace GardenPlanner_Web.Controllers {
	[Authorize(Roles = "UserAdmin,Admin,Employee")]
	public class VehiclesController : Controller {
		private readonly AgendaDbContext _context;
		private readonly Utilities _utilities;

		public VehiclesController(
			AgendaDbContext context,
			Utilities utilities) {
			_context = context;
			_utilities = utilities;
		}

		// GET: Vehicles
		public async Task<IActionResult> Index() {
			return View(await _context.Vehicles.Where(v => v.Id != "-" && v.Deleted == null).ToListAsync());
		}

		// GET: Vehicles/Details/5
		public async Task<IActionResult> Details(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles
				 .FirstOrDefaultAsync(m => m.Id == id);
			if (vehicle == null) {
				return NotFound();
			}

			return View(vehicle);
		}

		// GET: Vehicles/Create
		public IActionResult Create() {
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>();
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>();
			return View();
		}

		// POST: Vehicles/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Created,Deleted,LicencePlate,VehicleType,Brand,Model,LoadCapacity,WeightCapacity,FuelType,ImageUrl,IsManuel,IsInUse")] Vehicle vehicle) {
			if (!ModelState.IsValid) {
				ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>();
				ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>();
				return View(vehicle);
			}

			_context.Add(vehicle);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: Vehicles/Edit/5
		public async Task<IActionResult> Edit(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle == null) {
				return NotFound();
			}
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>(vehicle.VehicleType);
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>(vehicle.FuelType);
			return View(vehicle);
		}

		// POST: Vehicles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Created,Deleted,LicencePlate,VehicleType,Brand,Model,LoadCapacity,WeightCapacity,FuelType,ImageUrl,IsManuel,IsInUse")] Vehicle vehicle) {
			if (id != vehicle.Id) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(vehicle);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) {
					if (!VehicleExists(vehicle.Id)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["VehicleTypeItems"] = _utilities.GetEnumSelectList<VehicleType>(vehicle.VehicleType);
			ViewData["FuelTypeItems"] = _utilities.GetEnumSelectList<FuelType>(vehicle.FuelType);
			return View(vehicle);
		}

		// GET: Vehicles/Delete/5
		public async Task<IActionResult> Delete(string id) {
			if (id == null) {
				return NotFound();
			}

			var vehicle = await _context.Vehicles
				 .FirstOrDefaultAsync(m => m.Id == id);
			if (vehicle == null) {
				return NotFound();
			}

			return View(vehicle);
		}

		// POST: Vehicles/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id) {
			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle != null) {
				//_context.Vehicles.Remove(vehicle);
				vehicle.Deleted = DateTime.Now;
				_context.Update(vehicle);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool VehicleExists(string id) {
			return _context.Vehicles.Any(e => e.Id == id);
		}
	}
}
