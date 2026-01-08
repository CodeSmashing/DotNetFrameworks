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

	/// <summary>
	/// Converteert een <see cref="AgendaUserDTO"/> naar een nieuwe instance
	/// van <see cref="AgendaUser"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model dat geconverteerd moet worden.
	/// </param>
	/// <returns>
	/// Een nieuwe instantie van <see cref="AgendaUser"/> gevuld met de waarden
	/// uit de DTO, of <see langword="null"/> als <paramref name="model"/> null is.
	/// </returns>
	public static AgendaUser? ToModel(this AgendaUserDTO? model) {
		return model == null ? null : new() {
			FirstName = model.FirstName,
			LastName = model.LastName
		};
	}

	/// <summary>
	/// Kopieert de gegevens van een <see cref="AgendaUserDTO"/> naar een 
	/// bestaande instantie van <see cref="AgendaUser"/>.
	/// </summary>
	/// <param name="model">
	/// Het bron-model waarvan de gegevens worden overgenomen.
	/// </param>
	/// <param name="target">
	/// Het bestaande doel-model dat wordt bijgewerkt met de gegevens uit <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this AgendaUserDTO model, AgendaUser target) {
		target.FirstName = model.FirstName;
		target.LastName = model.LastName;
	}
}
