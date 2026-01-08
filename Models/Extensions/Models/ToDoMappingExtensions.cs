using Models.DTO;
namespace Models.Extensions.Models;

/// <summary>
/// Bevat extensiemethoden voor het transformeren van <see cref="ToDo"/>
/// en <see cref="ToDoDTO"/> entiteiten, waaronder het aanmaken van nieuwe
/// modelinstanties, het converteren naar DTO's en het bijwerken van bestaande
/// modellen op basis van DTO-gegevens.
/// </summary>
public static class ToDoMappingExtensions {
	/// <summary>
	/// Converteert een <see cref="ToDo"/> entiteit model naar een
	/// <see cref="ToDoDTO"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="ToDoDTO"/>, 
	/// of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static ToDoDTO? ToDTO(this ToDo? model) {
		return model == null ? null : new() {
			GlobalId = model.Id,
			AppointmentId = model.AppointmentId,
			Description = model.Description
		};
	}

	/// <summary>
	/// Converteert een <see cref="ToDoDTO"/> naar een nieuwe instance
	/// van <see cref="ToDo"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="ToDo"/> gevuld met de waarden
	/// uit de DTO, of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static ToDo? ToModel(this ToDoDTO? model) {
		return model == null ? null : new() {
			AppointmentId = model.AppointmentId,
			Description = model.Description
		};
	}

	/// <summary>
	/// Kopieert de gegevens van een <see cref="ToDoDTO"/> naar een 
	/// bestaande instantie van <see cref="ToDo"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model waarvan de gegevens worden overgenomen.
	/// </param>
	/// <param name="target">
	/// Het bestaande doel-model dat wordt bijgewerkt met de gegevens uit <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this ToDoDTO model, ToDo target) {
		target.AppointmentId = model.AppointmentId;
		target.Description = model.Description;
	}
}
