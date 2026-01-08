using Models.DTO;
namespace Models.Extensions.Models;

/// <summary>
/// Bevat extensiemethoden voor het transformeren van <see cref="Vehicle"/>
/// en <see cref="VehicleDTO"/> entiteiten, waaronder het aanmaken van nieuwe
/// modelinstanties, het converteren naar DTO's en het bijwerken van bestaande
/// modellen op basis van DTO-gegevens.
/// </summary>
public static class VehicleMappingExtensions {
	/// <summary>
	/// Converteert een <see cref="Vehicle"/> entiteit model naar een
	/// <see cref="VehicleDTO"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="VehicleDTO"/>, 
	/// of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static VehicleDTO? ToDTO(this Vehicle? model) {
		return model == null ? null : new() {
			GlobalId = model.Id,
			LicencePlate = model.LicencePlate,
			Brand = model.Brand,
			Model = model.Model,
			VehicleType = model.VehicleType,
			LoadCapacity = model.LoadCapacity,
			WeightCapacity = model.WeightCapacity,
			FuelType = model.FuelType
		};
	}
}
