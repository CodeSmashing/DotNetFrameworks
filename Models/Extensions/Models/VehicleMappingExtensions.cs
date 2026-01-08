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

	/// <summary>
	/// Converteert een <see cref="VehicleDTO"/> naar een nieuwe instance
	/// van <see cref="Vehicle"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="Vehicle"/> gevuld met de waarden
	/// uit de DTO, of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static Vehicle? ToModel(this VehicleDTO? model) {
		return model == null ? null : new() {
			LicencePlate = model.LicencePlate,
			Brand = model.Brand,
			Model = model.Model,
			VehicleType = model.VehicleType,
			LoadCapacity = model.LoadCapacity,
			WeightCapacity = model.WeightCapacity,
			FuelType = model.FuelType
		};
	}

	/// <summary>
	/// Kopieert de gegevens van een <see cref="VehicleDTO"/> naar een 
	/// bestaande instantie van <see cref="Vehicle"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model waarvan de gegevens worden overgenomen.
	/// </param>
	/// <param name="target">
	/// Het bestaande doel-model dat wordt bijgewerkt met de gegevens uit <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this VehicleDTO model, Vehicle target) {
		target.LicencePlate = model.LicencePlate;
		target.Brand = model.Brand;
		target.Model = model.Model;
		target.VehicleType = model.VehicleType;
		target.LoadCapacity = model.LoadCapacity;
		target.WeightCapacity = model.WeightCapacity;
		target.FuelType = model.FuelType;
	}
}
