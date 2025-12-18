using AspNetCore.Unobtrusive.Ajax;
using GardenPlanner_Web.Extensions;
using GardenPlanner_Web.Properties;
using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Models;
using Models.CustomServices;
using Models.CustomValidation;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

// Databasecontext
builder.Services.AddDbContext<AgendaDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("AgendaDbContextConnection")
		?? throw new InvalidOperationException("Connection string 'AgendaDbContextConnection' not found.")
	));

// Identiteitssysteem
builder.Services
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

// Registreer de Ajax (Unobtrusive) service
builder.Services.AddUnobtrusiveAjax();

// Aangepaste app-instellingen
builder.Services
	.AddOptions<GlobalAppSettings>()
	.Bind(builder.Configuration.GetSection("GlobalAppSettings"))
	.Configure(options => {
		options.DefaultCookieLifespan = DateTime.UtcNow.AddDays(12);
		options.EnvironmentName = builder.Environment.EnvironmentName;
		options.DefaultLanguageCode ??= "nl";
	})
	.ValidateOnStart(); // CRITIEK: De app start niet als de instellingen fout zijn;

// Maakt het mogelijk om het object met de ruwe instellingen direct te injecteren
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GlobalAppSettings>>().Value);

builder.Services.AddLogging();

// Lokalisatie
builder.Services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options => {
	CultureInfo[] supportedCultures = {
		new("en-US"),
		new("nl-BE"),
		new("fr-FR"),
		new("en"),
		new("nl"),
		new("fr")
	};

	options.DefaultRequestCulture = new RequestCulture(culture: "nl-BE", uiCulture: "nl-BE");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});

// MVC & Views met lokalisatie
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services
	.AddMvc()
	.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
	.AddDataAnnotationsLocalization(options => {
	 options.DataAnnotationLocalizerProvider = (type, factory) =>
		 factory.Create(typeof(Models.SharedResource));
 });

// Voor de configuratie van Restful API's
builder.Services.AddControllers();

// Voor het gebruik van Swagger
builder.Services.AddSwaggerGen(action => {
	action.SwaggerDoc("v1", new OpenApiInfo {
		Title = "GardenPlanner_Web",
		Version = "v1"
	});
});

// Utilities
builder.Services.AddTransient<Utilities>();

var app = builder.Build();

// Vul de database met data
using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;
	try {
		await AgendaDbContext.Seeder(services);
	} catch (Exception ex) {
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Configureer de HTTP-aanvraagpijplijn.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// De standaardwaarde voor HSTS is 30 dagen. Mogelijk wilt u dit wijzigen voor productiescenario's; zie https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
} else {
	app.UseSwagger();
	app.UseSwaggerUI(action => action.SwaggerEndpoint("/swagger/v1/swagger.json", "GardenPlanner_Web v1"));
}

app.UseHttpsRedirection();

// Lokalisatie middleware (Moet voor Routing/Authentication komen)
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

// Ajax middleware (Moet voor Routing komen)
app.UseUnobtrusiveAjax();

app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Authenticatie & autorisatie
app.UseAuthentication();
app.UseAuthorization();

// UseUserIdResolver middleware (Moet voor Endpoints komen)
app.UseUserIdResolver();

// Eindpunten
app.MapStaticAssets();
app.MapControllers();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages();

app.Run();
