using Models.CustomValidation;
using Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Vehicle {
		public int Id {
			get; set;
		}

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted
		{
			get; set;
		} = DateTime.MaxValue;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "De nummer plaat moet en mag enkel 9 characters bevatten")]
		[Display(Name = "Nummer plaat")]
		public string LicencePlate {
			get; set;
		} = null!; // Specifically Belgian plates (e.g. format of 1-ABC-111)

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
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Het model moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Model")]
		public string Model {
			get; set;
		} = null!;

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

		[Display(Name = "Afbeelding")]
		public string? ImageUrl {
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
		} = false;

		public override string ToString() {
			return $"Vehicle {Id} (Licence plate: {LicencePlate})";
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
					IsInUse = false,
					Deleted = DateTime.MaxValue
				},
				new() {
					LicencePlate = "3-XYZ-987",
					VehicleType = VehicleType.Pickup,
					ImageUrl = "https://evoximages.com/wp-content/uploads/2021/12/Ford-F-150-High-Quality-Car-Automotive-Stock-Photos-Images.png",
					Brand = "Ford",
					Model = "F-150",
					LoadCapacity = 13600.0,
					WeightCapacity = 1200.0,
					FuelType = FuelType.Benzine,
					IsManuel = false,
					IsInUse = true,
					Deleted = DateTime.MaxValue
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
					Deleted = DateTime.MaxValue
				},
				new() {
					LicencePlate = "7-GHI-789",
					VehicleType = VehicleType.Truck,
					ImageUrl = "https://media.istockphoto.com/id/1165674057/photo/man-tgs.jpg?s=1024x1024&w=is&k=20&c=MNB76hjjYbWT3MfMno3y4kfGkr2bqacBhrVJVjeMVkA=",
					Brand = "MAN",
					Model = "TGS",
					LoadCapacity = 28000.0,
					WeightCapacity = 20000.0,
					FuelType = FuelType.Diesel,
					IsManuel = true,
					IsInUse = true,
					Deleted = DateTime.MaxValue
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
					IsInUse = false,
					Deleted = DateTime.MaxValue
				}
			};
		}
	}
}
