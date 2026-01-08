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
}
