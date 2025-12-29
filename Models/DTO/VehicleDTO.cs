using Models.Enums;

namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een voertuig in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class VehicleDTO {
		/// <summary>
		/// De unieke identificatie van het voertuig.
		/// </summary>
		public required string Id {
			get; set;
		}

		/// <summary>
		/// De kentekenplaat van het voertuig
		/// </summary>
		public required string LicencePlate {
			get; set;
		}

		/// <summary>
		/// Het type van het voertuig
		/// </summary>
		public required VehicleType VehicleType {
			get; set;
		}

		/// <summary>
		/// Het merk van het voertuig
		/// </summary>
		public required string Brand {
			get; set;
		}

		/// <summary>
		/// Het model van het voertuig
		/// </summary>
		public required string Model {
			get; set;
		}

		/// <summary>
		/// De laadcapaciteit van het voertuig
		/// </summary>
		public required double LoadCapacity {
			get; set;
		}

		/// <summary>
		/// De gewicht capaciteit van het voertuig
		/// </summary>
		public required double WeightCapacity {
			get; set;
		}

		/// <summary>
		/// Het type brandstof voor het voertuig
		/// </summary>
		public required FuelType FuelType {
			get; set;
		}
	}
}