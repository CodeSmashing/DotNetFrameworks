using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.CustomServices;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AgendaDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AgendaDbContextConnection' not found.");
var totpProvider = typeof(PasswordlessLoginTotpTokenProvider<>).MakeGenericType(typeof(AgendaUser));

// Add services to the container.
builder.Services
	.AddDbContext<AgendaDbContext>()
	.AddLogging()
	.AddDefaultIdentity<AgendaUser>(options => {
		options.Password.RequireDigit = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Password.RequiredLength = 3;
		options.Password.RequiredUniqueChars = 1;
		options.Tokens.PasswordResetTokenProvider = "PasswordlessLoginTotpProvider";
		options.User.RequireUniqueEmail = true;
		options.SignIn.RequireConfirmedAccount = false;
	})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<AgendaDbContext>()
	.AddPasswordlessLoginTotpTokenProvider();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;
	try {
		// Seed the database
		await AgendaDbContext.Seeder(services);
	} catch (Exception ex) {
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Home}/{action=Index}/{id?}")
	 .WithStaticAssets();

app.MapRazorPages();

app.Run();
