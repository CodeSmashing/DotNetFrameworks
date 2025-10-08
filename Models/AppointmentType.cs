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

        static public AppointmentType Dummy = new AppointmentType { Name = "-", Description = "-", Deleted = DateTime.MaxValue };
        // seeding data
        public static List<AppointmentType> SeedingData()
        {
            var list = new List<AppointmentType>();
            list.AddRange(list = new List<AppointmentType>
            {
                // Voeg een dummy AppointmentType toe
                Dummy,

                // Voeg enkele voorbeeld AppointmentTypes toe
                new AppointmentType { Name = "Onderhoud", Description = "Algemeen onderhoud"},
                new AppointmentType { Name = "Aanleg", Description = "Aanleggen van tuin" },
                new AppointmentType { Name = "Kennismaking", Description = "Kennismaking klant" }
            });
            return list;

        }
    }
}