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
}
