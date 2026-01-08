using Models.DTO;
namespace Models.Extensions.Models;

/// <summary>
/// Bevat extensiemethoden voor het transformeren van <see cref="AppointmentType"/>
/// en <see cref="AppointmentTypeDTO"/> entiteiten, waaronder het aanmaken van nieuwe
/// modelinstanties, het converteren naar DTO's en het bijwerken van bestaande
/// modellen op basis van DTO-gegevens.
/// </summary>
public static class AppointmentTypeMappingExtensions {
	/// <summary>
	/// Converteert een <see cref="AppointmentType"/> entiteit model naar een
	/// <see cref="AppointmentTypeDTO"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="AppointmentTypeDTO"/>, 
	/// of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static AppointmentTypeDTO? ToDTO(this AppointmentType? model) {
		return model == null ? null : new() {
			GlobalId = model.Id,
			Name = model.Name,
			Description = model.Description
		};
	}

	/// <summary>
	/// Converteert een <see cref="AppointmentTypeDTO"/> naar een nieuwe instance
	/// van <see cref="AppointmentType"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="AppointmentType"/> gevuld met de waarden
	/// uit de DTO, of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static AppointmentType? ToModel(this AppointmentTypeDTO? model) {
		return model == null ? null : new() {
			Name = model.Name,
			Description = model.Description
		};
	}

	/// <summary>
	/// Kopieert de gegevens van een <see cref="AppointmentTypeDTO"/> naar een 
	/// bestaande instantie van <see cref="AppointmentType"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model waarvan de gegevens worden overgenomen.
	/// </param>
	/// <param name="target">
	/// Het bestaande doel-model dat wordt bijgewerkt met de gegevens uit <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this AppointmentTypeDTO model, AppointmentType target) {
		target.Name = model.Name;
		target.Description = model.Description;
	}
}
