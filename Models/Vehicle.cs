using Models.CustomValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Vehicle {
		public int Id {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "De nummer plaat moet en mag enkel 9 characters bevatten")]
		[Display(Name = "Nummer plaat")]
		public string LicencePlate {
			get; set;
		} = string.Empty; // Specifically Belgian plates (e.g. format of 1-ABC-111)

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Type voertuig")]
		[CustomValidation(typeof(EnumValidation), nameof(EnumValidation.ValidateEnum))]
		public VehicleType VehicleType {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Het merk moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Merk")]
		public string Brand {
			get; set;
		} = string.Empty;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Het model moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Model")]
		public string Model {
			get; set;
		} = string.Empty;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Laad capaciteit")]
		[Range(0.0, 9999999.99)]
		[RegularExpression(@"^\\d+(\\.\\d{1,2})?$")]
		public double LoadCapacity {
			get; set;
		} // In liters

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Gewicht capaciteit")]
		[Range(0.0, 9999999.99)]
		[RegularExpression(@"^\\d+(\\.\\d{1,2})?$")]
		public double WeightCapacity {
			get; set;
		} // In kilograms

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Brandstof type")]
		[CustomValidation(typeof(EnumValidation), nameof(EnumValidation.ValidateEnum))]
		public FuelType FuelType {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Is manueel")]
		public bool IsManuel {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Is in gebruik")]
		public bool IsInUse {
			get; set;
		}

		[ForeignKey("AgendaUser")]
		[Display(Name = "Werknemer ID")]
		public int? EmployeeId {
			get; set;
		}

		[Display(Name = "Werknemer")]
		public AgendaUser? Employee {
			get; set;
		}

		public override string ToString() {
			return $"Vehicle {Id} (Licence plate: {LicencePlate}) in use by: {Employee?.FirstName} {Employee?.LastName}";
		}

		// Seeding data
		public static List<Vehicle> SeedingData() {
			return new() {
				new() {
					LicencePlate = "0-ABC-123",
					VehicleType = VehicleType.Truck,
					Brand = "Volvo",
					Model = "FH16",
					LoadCapacity = 26000.0,  // in liters
					WeightCapacity = 18000.0, // in kilograms
					FuelType = FuelType.Diesel,
					IsManuel = false,
					IsInUse = false
				},
				new() {
					LicencePlate = "3-XYZ-987",
					VehicleType = VehicleType.Pickup,
					Brand = "Ford",
					Model = "F-150",
					LoadCapacity = 13600.0,
					WeightCapacity = 1200.0,
					FuelType = FuelType.Benzine,
					IsManuel = false,
					IsInUse = true,
					EmployeeId = 2
				},
				new() {
					LicencePlate = "5-JKL-456",
					VehicleType = VehicleType.Personal,
					Brand = "Toyota",
					Model = "Corolla",
					LoadCapacity = 5000.0,
					WeightCapacity = 1500.0,
					FuelType = FuelType.Benzine,
					IsManuel = true,
					IsInUse = true,
					EmployeeId = 3
				},
				new() {
					LicencePlate = "7-GHI-789",
					VehicleType = VehicleType.Truck,
					Brand = "MAN",
					Model = "TGS",
					LoadCapacity = 28000.0,
					WeightCapacity = 20000.0,
					FuelType = FuelType.Diesel,
					IsManuel = true,
					IsInUse = true,
					EmployeeId = 4
				},
				new() {
					LicencePlate = "9-MNO-321",
					VehicleType = VehicleType.Personal,
					Brand = "Honda",
					Model = "Civic",
					LoadCapacity = 4500.0,
					WeightCapacity = 1400.0,
					FuelType = FuelType.Benzine,
					IsManuel = false,
					IsInUse = false
				}
			};
		}
	}
}
