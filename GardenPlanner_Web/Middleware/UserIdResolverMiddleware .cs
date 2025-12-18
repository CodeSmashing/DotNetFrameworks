using Microsoft.AspNetCore.Identity;
using Models;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace GardenPlanner_Web.Middleware {
	/// <summary>
	/// Middleware voor het efficiënt vaststellen van de gebruikers-ID 
	/// om onnodige database-aanvragen te beperken.
	/// </summary>
	public class UserIdResolverMiddleware {
		private readonly RequestDelegate _next;
		private readonly ConcurrentDictionary<string, string> _cacheUsers = new(
			[KeyValuePair.Create(AgendaUser.Dummy.Email!, AgendaUser.Dummy.Id)]
		);

		/// <summary>
		/// Initialiseert een nieuwe instantie van de <see cref="UserIdResolverMiddleware"/> klasse.
		/// </summary>
		/// <param name="next">De volgende delegate in de HTTP-aanvraagpijplijn.</param>
		public UserIdResolverMiddleware(RequestDelegate next) {
			_next = next;
		}

		/// <summary>
		/// Verwerkt een HTTP-aanvraag asynchroon om de gebruikers-ID vast te stellen en te cachen.
		/// </summary>
		/// <param name="httpContext">De huidige HTTP-context voor de aanvraag.</param>
		/// <param name="userManager">De manager voor gebruikersbeheer, geïnjecteerd per scope.</param>
		/// <returns>
		/// Een <see cref="Task"/> die de asynchrone bewerking representeert.
		/// </returns>
		public async Task InvokeAsync(HttpContext httpContext, UserManager<AgendaUser> userManager) {
			// Probeer de ID en email direct uit de claims te halen (geen DB look-up)
			string? userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			string? userEmail = httpContext.User.FindFirstValue(ClaimTypes.Email);

			// Alleen actie ondernemen als we de ID nog NIET hebben uit de claims
			if (httpContext.User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(userId)) {
				// Probeer eerst de cache, als dat faalt, ga naar de DB
				if (!_cacheUsers.TryGetValue(userEmail, out userId)) {
					var user = await userManager.FindByEmailAsync(userEmail);
					userId = user?.Id;

					if (!string.IsNullOrEmpty(userId)) {
						_cacheUsers[userEmail] = userId;
					}
				}
			}

			// Veilig wegschrijven naar HttpContext.Items
			if (!string.IsNullOrEmpty(userId)) {
				httpContext.Items["UserId"] = userId;
			}

			// Roep de volgende delegate/middleware aan in de pijplijn
			await _next(httpContext);
		}
	}
}
