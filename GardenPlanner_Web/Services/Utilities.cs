using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GardenPlanner_Web.Services {
	public class Utilities {
		private readonly IHtmlLocalizer<SharedResource> _localizer;

		public Utilities(IHtmlLocalizer<SharedResource> localizer) {
			_localizer = localizer;
		}

		public List<SelectListItem> GetEnumSelectList<T>(Enum? selectedValue = null) where T : notnull {
			List<SelectListItem> list = [];
			
			// Placeholder only if no selected value
			if (selectedValue == null) {
				list.Add(new() {
					Value = "",
					Text = _localizer["SelectListPlaceholder"].Value,
					Selected = true
				});
			}

			foreach (Enum item in Enum.GetValues(typeof(T))) {
				list.Add(new() {
					Value = item.ToString(),
					Text = _localizer[item.ToString()].Value,
					Selected = (selectedValue?.ToString() == item.ToString())
				});
			}
			return list;
		}
	}
}
