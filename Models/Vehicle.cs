using Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Vehicle {
		public static Vehicle Dummy;

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		[Display(Name = "Created")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Deleted")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime? Deleted {
			get; set;
		}

		[StringLength(9, MinimumLength = 9)]
		[Display(Name = "LicencePlate", ResourceType = typeof(Resources.Vehicle))]
		public required string LicencePlate {
			get; set;
		} // Specifically Belgian plates (e.g. format of 1-ABC-111)

		[Display(Name = "VehicleType", ResourceType = typeof(Resources.Vehicle))]
		public required VehicleType VehicleType {
			get; set;
		}

		[StringLength(50, MinimumLength = 3)]
		[Display(Name = "Brand", ResourceType = typeof(Resources.Vehicle))]
		public required string Brand {
			get; set;
		}

		[StringLength(50, MinimumLength = 3)]
		[Display(Name = "Model", ResourceType = typeof(Resources.Vehicle))]
		public required string Model {
			get; set;
		}

		[Display(Name = "LoadCapacity", ResourceType = typeof(Resources.Vehicle))]
		[Range(0.0, 9999999.99)]
		public required double LoadCapacity {
			get; set;
		} // In liters

		[Display(Name = "WeightCapacity", ResourceType = typeof(Resources.Vehicle))]
		[Range(0.0, 9999999.99)]
		public required double WeightCapacity {
			get; set;
		} // In kilograms

		[Display(Name = "FuelType", ResourceType = typeof(Resources.Vehicle))]
		public required FuelType FuelType {
			get; set;
		}

		[Display(Name = "ImageUrl", ResourceType = typeof(Resources.Vehicle))]
		public string? ImageUrl {
			get; set;
		}

		[Display(Name = "IsManuel", ResourceType = typeof(Resources.Vehicle))]
		public bool IsManuel {
			get; set;
		} = false;

		[Display(Name = "IsInUse", ResourceType = typeof(Resources.Vehicle))]
		public bool IsInUse {
			get; set;
		} = false;

		public override string ToString() {
			return string.Format(Resources.Vehicle.ToString, Id, LicencePlate);
		}

		public static Vehicle[] SeedingData() {
			Dummy = new() {
				Id = "-",
				Created = new DateTime(2000, 1, 1),
				LicencePlate = string.Empty,
				Brand = string.Empty,
				Model = string.Empty,
				VehicleType = VehicleType.Personal,
				LoadCapacity = 0.0,
				WeightCapacity = 0.0,
				FuelType = FuelType.Diesel,
			};

			return [
				// Add a dummy Vehicle
				Dummy,
				
				// Add a few example Vehicles
				new() {
					LicencePlate = "0-ABC-123",
					VehicleType = VehicleType.Truck,
					ImageUrl = "https://assets.volvo.com/is/image/VolvoInformationTechnologyAB/volvo-fh16-cgi-exterior-1?qlt=82&wid=1024&ts=1705310176284&dpr=off&fit=constrain&fmt=png-alpha",
					Brand = "Volvo",
					Model = "FH16",
					LoadCapacity = 26000.0,
					WeightCapacity = 18000.0,
					FuelType = FuelType.Diesel,
					IsManuel = false,
					IsInUse = false
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
					IsInUse = true
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
					IsInUse = true
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
					IsInUse = true
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
			];
		}
	}

	/// <summary>
	/// Representeert een voertuig dat gebruikt word voor locale opslag synchronisatie, met een expliciet gezette identificatiecode.
	/// </summary>
	/// <remarks><see cref="LocalVehicle"/> breidt <see cref="Vehicle"/> uit door een <c>Id</c> eigenschap
	/// toe te voegen die niet door de database wordt gegenereerd. Dit is handig in scenario's waarin voertuigen
	/// lokaal moeten worden gevolgd of gesynchroniseerd en de identificaties door de applicatie in plaats van
	/// de database worden toegewezen.</remarks>
	public class LocalVehicle : Vehicle {
		/// <summary>
		/// Hiermee wordt de unieke identificatiecode voor de entiteit opgehaald of ingesteld.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public new string Id {
			get; private set;
		} = Guid.NewGuid().ToString();
	}
}
