using Models;

using (var context = new AgendaDbContext())
{
    AgendaDbContext.Seeder(context);

    // Haal alle appointments op en toon deze
    var alleAppointments = context.Appointments;
    var alleTodos = context.Todos;
    Console.WriteLine("Alle afspraken: ");
    foreach (var appointment in alleAppointments)
    {
        Console.WriteLine("- " + appointment);
        foreach (var todo in alleTodos)
        {
            if (todo.AppointmentId == appointment.Id)
                Console.WriteLine("   * " + todo);
        }
    }


    // Haal de appointments op die niet verwijderd zijn
    // Gebruik Where met een gedelegeerde functie
    var appointments = context.Appointments.Where(NotDeleted);
    bool NotDeleted(Appointment a)
    {
        return a.Deleted > DateTime.Now;
    }

    // Doe exact hetzelfde met een anonieme delegate
    appointments = context.Appointments
                          .Where(delegate (Appointment a) { return a.Deleted > DateTime.Now; });

    // Doe weer exact hetzelfde met een lambda-expressie
    appointments = context.Appointments.Where(a => a.Deleted > DateTime.Now
                                                && a.From > DateTime.Now)
                                        .OrderBy(a => a.Title);

    // Toon de gefilterde appointments
    Console.WriteLine("\nAlleen afspraken die niet verwijderd werden:");
    foreach (var appointment in appointments)
    {
        Console.WriteLine(appointment);
    }

    // Toevoegen van een afspraak
    //Appointment nieuweAfspraak = new Appointment()
    //{
    //    Title = "Tandardsbezoek",
    //    Description = "Dat gaat pijn doen",
    //    From = DateTime.Now.AddDays(7)
    //};
    //context.Add(nieuweAfspraak);   // hetzelfde als context.Appointments.Add(nieuweAfspraak);
    //context.SaveChanges();
    //Console.WriteLine("\nEen afspraak werd toegevoegd:");
    //foreach (var appointment in appointments)
    //{
    //    Console.WriteLine(appointment);
    //}

    
}