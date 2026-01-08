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

	/// <summary>
	/// Converteert een <see cref="AppointmentDTO"/> naar een nieuwe instance
	/// van <see cref="Appointment"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="Appointment"/> gevuld met de waarden
	/// uit de DTO, of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static Appointment? ToModel(this AppointmentDTO? model) {
		return model == null ? null : new() {
			AgendaUserId = model.AgendaUserId,
			Date = model.Date,
			Title = model.Title,
			Description = model.Description,
			AppointmentTypeId = model.AppointmentTypeId
		};
	}

	/// <summary>
	/// Kopieert de gegevens van een <see cref="AppointmentDTO"/> naar een 
	/// bestaande instantie van <see cref="Appointment"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model waarvan de gegevens worden overgenomen.
	/// </param>
	/// <param name="target">
	/// Het bestaande doel-model dat wordt bijgewerkt met de gegevens uit <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this AppointmentDTO model, Appointment target) {
		target.AgendaUserId = model.AgendaUserId;
		target.Date = model.Date;
		target.Title = model.Title;
		target.Description = model.Description;
		target.AppointmentTypeId = model.AppointmentTypeId;
	}
}
