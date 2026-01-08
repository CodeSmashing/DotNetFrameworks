using Models.DTO;
namespace Models.Extensions.Models;

/// <summary>
/// Bevat extensiemethoden voor het transformeren van <see cref="AgendaUser"/>
/// en <see cref="AgendaUserDTO"/> entiteiten, waaronder het aanmaken van nieuwe
/// modelinstanties, het converteren naar DTO's en het bijwerken van bestaande
/// modellen op basis van DTO-gegevens.
/// </summary>
public static class AgendaUserMappingExtensions {
	/// <summary>
	/// Converteert een <see cref="AgendaUser"/> entiteit model naar een
	/// <see cref="AgendaUserDTO"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="AgendaUserDTO"/>, 
	/// of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static AgendaUserDTO? ToDTO(this AgendaUser? model) {
		return model == null ? null : new() {
			GlobalId = model.Id,
			FirstName = model.FirstName,
			LastName = model.LastName
		};
	}
}
