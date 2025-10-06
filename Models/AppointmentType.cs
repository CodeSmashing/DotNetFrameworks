using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppointmentType
    {
        // Dummy instantie voor referentie
        static public AppointmentType Dummy = null;

        // [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public DateTime Deleted { get; set; } = DateTime.MaxValue;


        public override string ToString()
        {
            return $"{Id}: {Name} ({Description}) - Deleted: {Deleted}";
        }

        // Voorzie ook een seeding data
        public static List<AppointmentType> SeedingData()
        {
            var list = new List<AppointmentType>();
            list.AddRange(list = new List<AppointmentType>
            {
                // Voeg een dummy AppointmentType toe
                new AppointmentType { Name = "-", Description = "-", Deleted = DateTime.MaxValue },

                // Voeg enkele voorbeeld AppointmentTypes toe
                new AppointmentType { Name = "Meeting", Description = "Business meeting"},
                new AppointmentType { Name = "Doctor", Description = "Doctor's appointment" },
                new AppointmentType { Name = "Holiday", Description = "Holiday event" },
                new AppointmentType { Name = "Conference", Description = "Professional conference" }
            });
            return list;

        }
    }
}