using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Appointment {
		private DateTime now = DateTime.Now + new TimeSpan(0, 1, 0);

		private DateTime _from = DateTime.Now + new TimeSpan(1, 0, 0, 0);

		public long Id {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[ForeignKey("AgendaUser")]
		[Display(Name = "Gebruiker ID")]
		public string AgendaUserId {
			get; set;
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Gebruiker")]
		public AgendaUser AgendaUser {
			get; set;
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Vanaf")]
		[DataType(DataType.DateTime)]
		public DateTime From {
			get => _from;
			set {
				if (value < now)
					_from = now;
				else
					_from = value;
			}
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Tot")]
		[DataType(DataType.DateTime)]
		public DateTime To {
			get; set;
		} = DateTime.Now + new TimeSpan(1, 1, 30, 0);

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "De titel moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Titel")]
		public string Title {
			get; set;
		} = string.Empty;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(2000, MinimumLength = 3, ErrorMessage = "De omschrijving moet minstens 3 characters en mag maximum 2000 characters bevatten")]
		[Display(Name = "Omschrijving")]
		[DataType(DataType.MultilineText)]
		public string Description {
			get; set;
		} = string.Empty;

		[Display(Name = "Aangemaakt")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted {
			get; set;
		} = DateTime.MaxValue;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Afspraak type ID")]
		[ForeignKey("AppointmentType")]
		public int AppointmentTypeId {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Afspraak type")]
		public AppointmentType AppointmentType {
			get; set;
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Is goedgekeurd")]
		public bool IsApproved {
			get; set;
		} = false;

		[Display(Name = "Is afgerond")]
		public bool IsCompleted {
			get; set;
		}

		public override string ToString() {
			return Id + "  Afspraak op " + From + " betreffende " + Title;
		}

		// Seeding data
		public static List<Appointment> SeedingData(List<AgendaUser> listUsers) {
			Random rnd = new();
			return new() {
				new() {
					AgendaUserId = listUsers[rnd.Next(listUsers.Count)].Id,
					Title = "Afspraak met Jan",
					Description = "Bespreking van het tuin ontwerp",
					From = DateTime.Now.AddDays(2).AddHours(10),
					To = DateTime.Now.AddDays(2).AddHours(11),
					AppointmentTypeId = 2 },

				new() {
					AgendaUserId = listUsers[rnd.Next(listUsers.Count)].Id,
					Title = "Onderhoud tuin bij Piet",
					Description = "Jaarlijks onderhoud van de tuin",
					From = DateTime.Now.AddDays(5).AddHours(9),
					To = DateTime.Now.AddDays(5).AddHours(10),
					AppointmentTypeId = 1 },

				new() {
					AgendaUserId = listUsers[rnd.Next(listUsers.Count)].Id,
					Title = "Kennismaking met Klaas",
					Description = "Eerste gesprek over mogelijke tuin projecten",
					From = DateTime.Now.AddDays(7).AddHours(14),
					To = DateTime.Now.AddDays(7).AddHours(15),
					AppointmentTypeId = 3 }
			};
		}
	}
}