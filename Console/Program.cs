using Models;

using (var context = new AgendaDbContext()) {
	/* Before we set the AgendaDbContext.Seeder() to use IServiceProvider:
	await AgendaDbContext.Seeder(context);

	// Haal alle appointments op en toon deze
	var alleAppointments = context.Appointments;
	var alleToDos = context.ToDos;
	Console.WriteLine("Alle afspraken: ");
	foreach (var appointment in alleAppointments) {
		Console.WriteLine("- " + appointment);
		foreach (var todo in alleToDos) {
			if (todo.AppointmentId == appointment.Id)
				Console.WriteLine("   * " + todo);
		}
	}
	*/


	// Haal de appointments op die niet verwijderd zijn
	// Gebruik Where met een gedelegeerde functie


	// Toevoegen van een afspraak
	//Appointment nieuweAfspraak = new Appointment()
	//{
	//    Title = "Tandartsbezoek",
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