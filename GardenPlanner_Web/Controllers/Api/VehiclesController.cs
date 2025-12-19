
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van voertuigen.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly AgendaDbContext _context;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="VehiclesController"/> klasse.
		/// </summary>
		/// <param name="context">
		/// De context voor voertuig beheer (dependency injection).
		/// </param>
		public VehiclesController(AgendaDbContext context) {
			_context = context;
		}

		// GET: api/Vehicles
		/// <summary>
		/// Haalt een lijst van alle voertuigen op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die een lijst met
		/// <see cref="Vehicle"/> objecten bevat.
		/// </returns>
		/// <response code="200">
		/// Retourneert de lijst met de voertuigen.
		/// </response>
		[HttpGet]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles() {
			return await _context.Vehicles.Where(v => v.Deleted == null).ToListAsync();
		}

		// GET: api/Vehicles/5
		/// <summary>
		/// Haalt een specifieke voertuig op basis van ID op.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin", "Employee".
		/// </remarks>
		/// <param name="id">
		/// De unieke identificatie (GUID of string) van de afspraak.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> met het gevraagde
		/// <see cref="Vehicle"/> object.
		/// </returns>
		/// <response code="200">
		/// Retourneert de gevraagde voertuig.
		/// </response>
		/// <response code="404">
		/// Indien de voertuig met de opgegeven ID niet gevonden is.
		/// </response>
		[HttpGet("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		public async Task<ActionResult<Vehicle>> GetVehicle(string id) {
			var vehicle = await _context.Vehicles.FindAsync(id);

			if (vehicle == null || vehicle.Deleted != null) {
				return NotFound();
			}

			return vehicle;
		}

		// PUT: api/Vehicles/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Werkt een bestaande voertuig bij.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "UserAdmin", "Admin".
		/// </remarks>
		/// <param name="id">
		/// De ID van de voertuig die moet worden bijgewerkt.
		/// </param>
		/// <param name="vehicle">
		/// De bijgewerkte voertuig gegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de voertuig succesvol is bijgewerkt (No Content).
		/// </response>
		/// <response code="400">
		/// Indien de opgegeven ID in de route niet overeenkomt met de
		/// ID in de body.
		/// </response>
		/// <response code="404">
		/// Indien de voertuig niet gevonden is.
		/// </response>
		[HttpPut("{id}")]
		[Authorize(Roles = "UserAdmin,Admin")]
		public async Task<IActionResult> PutVehicle(string id, Vehicle vehicle) {
			if (id != vehicle.Id) {
				return BadRequest();
			}

			_context.Entry(vehicle).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!VehicleExists(id)) {
					return NotFound();
				} else {
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Vehicles
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Maakt een nieuwe voertuig aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "Admin".
		/// </remarks>
		/// <param name="vehicle">
		/// Het <see cref="Vehicle"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// voertuig retourneert.
		/// </returns>
		/// <response code="201">
		/// Retourneert het aangemaakte item (Created).
		/// </response>
		/// <response code="400">
		/// Indien de invoergegevens ongeldig zijn.
		/// </response>
		/// <response code="409">
		/// Als de voertuig ID al bestaat in de database.
		/// </response>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle) {
			_context.Vehicles.Add(vehicle);
			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) {
				if (VehicleExists(vehicle.Id)) {
					return Conflict();
				} else {
					throw;
				}
			}

			return CreatedAtAction("GetVehicle", new {
				id = vehicle.Id
			}, vehicle);
		}

		// DELETE: api/Vehicles/5
		/// <summary>
		/// Verwijdert een voertuig op basis van ID. (Soft-delete)
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "Admin".
		/// </remarks>
		/// <param name="id">
		/// De ID van de voertuig die moet worden verwijderd.
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		/// <response code="204">
		/// Indien de voertuig succesvol is verwijderd (No Content).
		/// </response>
		/// <response code="404">
		/// Indien de voertuig niet gevonden is.
		/// </response>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteVehicle(string id) {
			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle == null) {
				return NotFound();
			}

			//_context.Vehicles.Remove(vehicle);
			vehicle.Deleted = DateTime.Now;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool VehicleExists(string id) {
			return _context.Vehicles.Any(e => e.Id == id);
		}
	}
}
