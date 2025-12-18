using GardenPlanner_Web.Middleware;

namespace GardenPlanner_Web.Extensions {
	/// <summary>
	/// Bevat uitbreiding-methoden voor het registreren van de <see cref="UseUserIdResolver"/> in de applicatiepijplijn.
	/// </summary>
	public static class MiddlewareExtensions {
		/// <summary>
		/// Voegt de <see cref="UseUserIdResolver"/> toe aan de HTTP-aanvraagpijplijn.
		/// </summary>
		/// <param name="builder">De <see cref="IApplicationBuilder"/> instantie.</param>
		/// <returns>
		/// De <see cref="IApplicationBuilder"/> zodat meerdere aanroepen gekoppeld kunnen worden.
		/// </returns>
		public static IApplicationBuilder UseUserIdResolver(this IApplicationBuilder builder) {
			return builder.UseMiddleware<UserIdResolverMiddleware>();
		}
	}
}
