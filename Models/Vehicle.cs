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
		[Display(Name = "Afbeelding")]
		public string? ImageUrl
		{
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
					ImageUrl = "https://assets.volvo.com/is/image/VolvoInformationTechnologyAB/volvo-fh16-cgi-exterior-1?qlt=82&wid=1024&ts=1705310176284&dpr=off&fit=constrain&fmt=png-alpha",
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
					ImageUrl = "https://www.ford.ca/cmslibs/content/dam/na/ford/en_ca/images/f-150/2025/jellybeans/F150_Jelly_Tremor1.png",
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
					ImageUrl = "https://kong-proxy-intranet.toyota-europe.com/c1-images/resize/ccis/680x680/zip/be/product-token/3b274324-851e-4d09-a5c2-9ef6d3f63538/vehicle/34c30a2d-010f-4707-a494-77106a004661/padding/50,50,50,50/image-quality/70/day-exterior-04_040.png",
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
					ImageUrl = "https://www.basworld.com/_next/image?url=https%3A%2F%2Fstatic.basworld.com%2Fphotos%2Fvehicle%2Fworld%2F1080%2Fused-Trekker-MAN-TGS-18.510-4X4-2020_336024_B9bQ6mgo6rZmI4xk69I7g6rF4HoGRxrUNPQL.jpg&w=1080&q=75",
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
					ImageUrl = "https://images.hgmsites.net/med/2025-honda-civic-sport-cvt-angular-front-exterior-view_100939989_m.webp",
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
