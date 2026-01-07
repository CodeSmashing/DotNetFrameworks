using Models.DTO;

namespace Models.Extensions.Models {
	/// <summary>
	/// Bevat extensiemethoden voor het transformeren van domeinmodellen naar
	/// Data Transfer Objects (DTO's).
	/// </summary>
	public static class DtoMappingExtensions {
		/// <summary>
		/// Converteert een <see cref="AgendaUser"/> domeinmodel naar een
		/// <see cref="AgendaUserDTO"/>.
		/// </summary>
		/// <param name="model">
		/// Het bron-model dat geconverteerd moet worden.
		/// </param>
		/// <returns>
		/// Een nieuwe instantie van <see cref="AgendaUserDTO"/>, 
		/// of <see langword="null"/> als het bron-model null is.
		/// </returns>
		public static AgendaUserDTO? ToDTO(this AgendaUser? model) {
			return model == null ? null : new() {
				GlobalId = model.Id,
				FirstName = model.FirstName,
				LastName = model.LastName
			};
		}

		/// <summary>
		/// Converteert een <see cref="Appointment"/> domeinmodel naar een
		/// <see cref="AppointmentDTO"/>.
		/// </summary>
		/// <param name="model">
		/// Het bron-model dat geconverteerd moet worden.
		/// </param>
		/// <returns>
		/// Een nieuwe instantie van <see cref="AppointmentDTO"/>, 
		/// of <see langword="null"/> als het bron-model null is.
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
		/// Converteert een <see cref="AppointmentType"/> domeinmodel naar een
		/// <see cref="AppointmentTypeDTO"/>.
		/// </summary>
		/// <param name="model">
		/// Het bron-model dat geconverteerd moet worden.
		/// </param>
		/// <returns>
		/// Een nieuwe instantie van <see cref="AppointmentTypeDTO"/>, 
		/// of <see langword="null"/> als het bron-model null is.
		/// </returns>
		public static AppointmentTypeDTO? ToDTO(this AppointmentType? model) {
			return model == null ? null : new() {
				GlobalId = model.Id,
				Name = model.Name,
				Description = model.Description
			};
		}

		/// <summary>
		/// Converteert een <see cref="Vehicle"/> domeinmodel naar een
		/// <see cref="VehicleDTO"/>.
		/// </summary>
		/// <param name="model">
		/// Het bron-model dat geconverteerd moet worden.
		/// </param>
		/// <returns>
		/// Een nieuwe instantie van <see cref="VehicleDTO"/>, 
		/// of <see langword="null"/> als het bron-model null is.
		/// </returns>
		public static VehicleDTO? ToDTO(this Vehicle? model) {
			return model == null ? null : new() {
				GlobalId = model.Id,
				LicencePlate = model.LicencePlate,
				Brand = model.Brand,
				Model = model.Model,
				VehicleType = model.VehicleType,
				LoadCapacity = model.LoadCapacity,
				WeightCapacity = model.WeightCapacity,
				FuelType = model.FuelType
			};
		}

		/// <summary>
		/// Converteert een <see cref="ToDo"/> domeinmodel naar een <see cref="ToDoDTO"/>.
		/// </summary>
		/// <param name="model">
		/// Het bron-model dat geconverteerd moet worden.
		/// </param>
		/// <returns>
		/// Een nieuwe instantie van <see cref="ToDoDTO"/>, 
		/// of <see langword="null"/> als het bron-model null is.
		/// </returns>
		public static ToDoDTO? ToDTO(this ToDo? model) {
			return model == null ? null : new() {
				GlobalId = model.Id,
				AppointmentId = model.AppointmentId,
				Description = model.Description
			};
		}
	}
}
