using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Todo
    {

        public long Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Klaar")]
        public bool Ready { get; set; } = false;


        // Foreign key naar Appointment
        [Required]
        [Display(Name = "Appointment")]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        static public Todo Dummy = new Todo { Title = "-", Ready = false, AppointmentId = 0};

        public override string ToString()
        {
            return $"{Id}: {Title} ({Ready})";
        }

        // seeding data
        public static List<Todo> SeedingData()
        {
            var list = new List<Todo>();
            list.AddRange(list = new List<Todo>
            {
                // Voeg een dummy Todo toe
                Dummy,

                // Voeg enkele voorbeeld Todos toe
                new Todo { Title = "Eerste Todo", Ready = false, AppointmentId = 1},
                new Todo { Title = "Tweede Todo", Ready = true, AppointmentId = 1},
                new Todo { Title = "Derde Todo", Ready = false , AppointmentId = 1},
            });
            return list;

        }
    }
}
