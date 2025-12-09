using GardenPlanner_Web.Properties;
using GardenPlanner_Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Models.CustomServices;
using Models.CustomValidation;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

// Database Context
builder.Services.AddDbContext<AgendaDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("AgendaDbContextConnection")
		?? throw new InvalidOperationException("Connection string 'AgendaDbContextConnection' not found.")
	));

// Identity System
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

// Custom App Settings
builder.Services
	.Configure<GlobalAppSettings>(builder.Configuration.GetSection("GlobalAppSettings"))
	.PostConfigure<GlobalAppSettings>(options => {
		options.DefaultCookieLifespan = DateTime.UtcNow.AddDays(12);
	});

// Allow injecting the raw settings object directly
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GlobalAppSettings>>().Value);

builder.Services.AddLogging();

// Localization
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

// MVC & Views with Localization
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services
	.AddMvc()
	.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
	.AddDataAnnotationsLocalization(options => {
	 options.DataAnnotationLocalizerProvider = (type, factory) =>
		 factory.Create(typeof(Models.SharedResource));
 });

// Utilities
builder.Services.AddTransient<Utilities>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;
	try {
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

// Localization Middleware (Must be before Routing/Auth)
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Endpoints
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages();

app.Run();
