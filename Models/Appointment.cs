using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Appointment
    {
        private DateTime now = DateTime.Now + new TimeSpan(0, 1, 0);
        private DateTime _from = DateTime.Now + new TimeSpan(1, 0, 0, 0);

        public long Id { get; set; } // = -1;  Niet nodig, wordt automatisch gegenereerd    

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
        public int AppointmentTypeId { get; set; } = AppointmentType.Dummy.Id; // Standaard naar de Dummy verwijzen

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
                new Appointment { Title = "Afspraak met Bob", From = DateTime.Now.AddDays(7), To = DateTime.Now.AddDays(7).AddHours(1) },
                new Appointment { Title = "Lunch met Alice", From = DateTime.Now.AddDays(12), To = DateTime.Now.AddDays(12).AddHours(2) },
                new Appointment { Title = "Projectbespreking", From = DateTime.Now.AddDays(15), To = DateTime.Now.AddDays(15).AddHours(3) }
                );

            return list;
        }
    }
}