using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.CustomServices;
using Models.DTO;
using System.Net.Mime;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert de API-eindpunten voor het beheren van voertuigen.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
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
		/// <see cref="VehicleDTO"/> objecten bevat.
		/// </returns>
		[HttpGet]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		[ProducesResponseType<IEnumerable<VehicleDTO>>(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetVehicles() {
			return Ok(await _context.Vehicles
				.Where(v => v.Deleted == null)
				.Select(v => v.ToDTO())
				.ToListAsync());
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
		/// <see cref="VehicleDTO"/> object.
		/// </returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "UserAdmin,Admin,Employee")]
		[ProducesResponseType<VehicleDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Vehicle>> GetVehicle(string id) {
			var vehicle = await _context.Vehicles.FindAsync(id);

			if (vehicle == null || vehicle.Deleted != null) {
				return NotFound();
			}

			return Ok(vehicle.ToDTO());
		}

		// PUT: api/Vehicles/5
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
		/// <param name="vehicleDTO">
		/// De bijgewerkte voertuig gegevens in de body van het verzoek.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpPut("{id}")]
		[Authorize(Roles = "UserAdmin,Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> PutVehicle(string id, VehicleDTO vehicleDTO) {
			if (id != vehicleDTO.Id) {
				return BadRequest();
			}

			var vehicle = await _context.Vehicles.FindAsync(id);
			if (vehicle == null) {
				return NotFound();
			}

			vehicle.LicencePlate = vehicleDTO.LicencePlate;
			vehicle.VehicleType = vehicleDTO.VehicleType;
			vehicle.Brand = vehicleDTO.Brand;
			vehicle.Model = vehicleDTO.Model;
			vehicle.LoadCapacity = vehicleDTO.LoadCapacity;
			vehicle.WeightCapacity = vehicleDTO.WeightCapacity;
			vehicle.FuelType = vehicleDTO.FuelType;
			_context.Entry(vehicle).State = EntityState.Modified;

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) when (!VehicleExists(id)) {
				return NotFound();
			}

			return NoContent();
		}

		// POST: api/Vehicles
		/// <summary>
		/// Maakt een nieuwe voertuig aan.
		/// </summary>
		/// <remarks>
		/// Vereist authenticatie. Gebruikers met een van de volgende
		/// rollen hebben toegang: 
		/// "Admin".
		/// </remarks>
		/// <param name="vehicleDTO">
		/// Het <see cref="VehicleDTO"/> object dat moet worden
		/// toegevoegd.
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{T}"/> die de zojuist aangemaakte
		/// voertuig retourneert.
		/// </returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType<VehicleDTO>(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<VehicleDTO>> PostVehicle(VehicleDTO vehicleDTO) {
			Vehicle vehicle = new() {
				LicencePlate = vehicleDTO.LicencePlate,
				VehicleType = vehicleDTO.VehicleType,
				Brand = vehicleDTO.Brand,
				Model = vehicleDTO.Model,
				LoadCapacity = vehicleDTO.LoadCapacity,
				WeightCapacity = vehicleDTO.WeightCapacity,
				FuelType = vehicleDTO.FuelType
			};

			_context.Vehicles.Add(vehicle);

			try {
				await _context.SaveChangesAsync();
			} catch (DbUpdateException) when (VehicleExists(vehicle.Id)) {
				return Conflict();
			}

			return CreatedAtAction(
				nameof(PostVehicle),
				new {
					id = vehicle.Id
				},
				vehicle.ToDTO());
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
		/// Een <see cref="ActionResult"/> die de status van de
		/// bewerking weergeeft.
		/// </returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeleteVehicle(string id) {
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
