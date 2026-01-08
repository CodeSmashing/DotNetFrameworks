using Models.DTO;
namespace Models.Extensions.Models;

/// <summary>
/// Bevat extensiemethoden voor het transformeren van <see cref="Appointment"/>
/// en <see cref="AppointmentDTO"/> entiteiten, waaronder het aanmaken van nieuwe
/// modelinstanties, het converteren naar DTO's en het bijwerken van bestaande
/// modellen op basis van DTO-gegevens.
/// </summary>
public static class AppointmentMappingExtensions {
	/// <summary>
	/// Converteert een <see cref="Appointment"/> entiteit model naar een
	/// <see cref="AppointmentDTO"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="AppointmentDTO"/>, 
	/// of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static AppointmentDTO? ToDTO(this Appointment? model) {
		return model == null ? null : new() {
			GlobalId = model.Id,
			AgendaUserId = model.AgendaUserId,
			Date = model.Date,
			Title = model.Title,
			Description = model.Description,
			AppointmentTypeId = model.AppointmentTypeId
		};
	}
}
