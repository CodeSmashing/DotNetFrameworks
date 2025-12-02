using GardenPlanner_Web.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Models.CustomServices;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AgendaDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AgendaDbContextConnection' not found.");
var totpProvider = typeof(PasswordlessLoginTotpTokenProvider<>).MakeGenericType(typeof(AgendaUser));

// Set configuration options
builder.Services
	.Configure<GlobalAppSettings>(builder.Configuration.GetSection("GlobalAppSettings"))
	.PostConfigure<GlobalAppSettings>(options => {
		options.DefaultCookieLifespan = DateTime.UtcNow.AddDays(12);
	})
	.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GlobalAppSettings>>().Value);

// Add services to the container.
builder.Services
	.AddDbContext<AgendaDbContext>()
	.AddLogging()
	.AddLocalization(options => {
		options.ResourcesPath = "Localization";
	})
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

builder.Services.AddMvc()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization();

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

// Adding middleware for localization
string[] supportedCultures = [
	"en-US",
	"nl-BE",
	"fr-FR",
	"en",
	"nl",
	"fr"
];
var localizationOptions = new RequestLocalizationOptions()
	.SetDefaultCulture(supportedCultures[1])
	.AddSupportedCultures(supportedCultures)
	.AddSupportedUICultures(supportedCultures);

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
