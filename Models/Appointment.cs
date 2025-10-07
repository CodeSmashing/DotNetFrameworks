using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Appointment
    {
        private DateTime now = DateTime.Now + new TimeSpan(0, 1, 0);
        private DateTime _from = DateTime.Now + new TimeSpan(1, 0, 0, 0);

        public long Id { get; set; } 

        [Required]
        [Display(Name = "Vanaf")]
        [DataType(DataType.DateTime)]
        public DateTime From
        {
            get => _from;
            set
            {
                if (value < now)
                    _from = now;
                else
                    _from = value;
            }
        }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Tot")]
        public DateTime To { get; set; } = DateTime.Now + new TimeSpan(1, 1, 30, 0);

        [Required]
        [Display(Name = "Titel")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Omschrijving")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Hele dag")]
        public bool AllDay { get; set; } = false;

        [Required]
        [Display(Name = "Aangemaakt")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Verwijderd")]
        public DateTime Deleted { get; set; } = DateTime.MaxValue;

        // Foreign key naar AppointmentType:  Zorg voor de juiste één-op-veel relatie
        [Required]
        [Display(Name = "Type")]
        [ForeignKey("AppointmentType")]
        public int AppointmentTypeId { get; set; } = AppointmentType.Dummy.Id;

        // Navigatie-eigenschap
        public AppointmentType? AppointmentType { get; set; }



        public override string ToString()
        {
            return Id + "  Afspraak op " + From + " betreffende " + Title;
        }


        public static List<Appointment> SeedingData()
        {
            var list = new List<Appointment>();
            list.AddRange(
                // Voeg een default-appointment toe
                new Appointment { Title = "-", Deleted = DateTime.Now },

                // Voeg enkele test-appointments toe
                new Appointment { Title = "Meeting with Bob", Description = "Discuss project updates", From = DateTime.Now.AddDays(2).AddHours(10), To = DateTime.Now.AddDays(2).AddHours(11), AppointmentTypeId = 1 },
                new Appointment { Title = "Doctor's Appointment", Description = "Annual check-up", From = DateTime.Now.AddDays(3).AddHours(9), To = DateTime.Now.AddDays(3).AddHours(10), AppointmentTypeId = 2 },
                new Appointment { Title = "Team Conference", Description = "Quarterly team meeting", From = DateTime.Now.AddDays(5).AddHours(14), To = DateTime.Now.AddDays(5).AddHours(16), AppointmentTypeId = 4 },
                new Appointment { Title = "Holiday", Description = "Family vacation", From = DateTime.Now.AddDays(10), To = DateTime.Now.AddDays(15), AllDay = true, AppointmentTypeId = 3 },
                new Appointment { Title = "Project Deadline", Description = "Submit final project report", From = DateTime.Now.AddDays(7).AddHours(17), To = DateTime.Now.AddDays(7).AddHours(18), AppointmentTypeId = 1 },
                new Appointment { Title = "Client Meeting", Description = "Meeting with client to discuss requirements", From = DateTime.Now.AddDays(4).AddHours(11), To = DateTime.Now.AddDays(4).AddHours(12), AppointmentTypeId = 1 },
                new Appointment { Title = "Webinar", Description = "Attend online marketing webinar", From = DateTime.Now.AddDays(6).AddHours(15), To = DateTime.Now.AddDays(6).AddHours(16), AppointmentTypeId = 4 },
                new Appointment { Title = "Dentist Appointment", Description = "Routine dental check-up", From = DateTime.Now.AddDays(8).AddHours(10), To = DateTime.Now.AddDays(8).AddHours(11), AppointmentTypeId = 2 }
                );

            return list;
        }
    }
}