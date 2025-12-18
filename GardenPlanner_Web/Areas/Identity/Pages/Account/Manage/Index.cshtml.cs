// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using GardenPlanner_Web.Extensions;
using GardenPlanner_Web.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System.ComponentModel.DataAnnotations;

namespace GardenPlanner_Web.Areas.Identity.Pages.Account.Manage {
	public class IndexModel : PageModel {
		private readonly UserManager<AgendaUser> _userManager;
		private readonly SignInManager<AgendaUser> _signInManager;
		private readonly AgendaDbContext _context;
		private readonly GlobalAppSettings _globalAppSettings;

		public IndexModel(
			UserManager<AgendaUser> userManager,
			SignInManager<AgendaUser> signInManager,
			AgendaDbContext context,
			GlobalAppSettings globalAppSettings) {
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_globalAppSettings = globalAppSettings;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string DisplayName {
			get; set;
		}

		public string FirstName {
			get; set;
		}

		public string LastName {
			get; set;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string StatusMessage {
			get; set;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input {
			get; set;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel {
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			//[Phone]
			//[Display(Name = "Phone number")]
			//public string PhoneNumber {
			//	get; set;
			//}

			[Display(Name = "Taal code")]
			public string LanguageCode {
				get; set;
			}
		}

		private async Task LoadAsync(AgendaUser user) {
			//var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			DisplayName = user.DisplayName;
			FirstName = user.FirstName;
			LastName = user.LastName;
			SelectList sl = new(Language.Languages.Where(l => l.Code != "-" && l.IsSystemLanguage), "Code", "Name", user.LanguageCode);
			ViewData["Languages"] = sl;

			Input = new InputModel {
				//PhoneNumber = phoneNumber
			};
		}

		public async Task<IActionResult> OnGetAsync() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{Request.HttpContext.GetUserId()}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{Request.HttpContext.GetUserId()}'.");
			}

			if (!ModelState.IsValid) {
				await LoadAsync(user);
				return Page();
			}

			//var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			//if (Input.PhoneNumber != phoneNumber) {
			//	var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
			//	if (!setPhoneResult.Succeeded) {
			//		StatusMessage = "Unexpected error when trying to set phone number.";
			//		return RedirectToPage();
			//	}
			//}

			// Update user language code if present
			if (user.LanguageCode != Input.LanguageCode) {
				user.LanguageCode = Input.LanguageCode;
				_context.Update(user);
				_context.SaveChanges();

				Response.Cookies.Append(
					CookieRequestCultureProvider.DefaultCookieName,
					CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Input.LanguageCode)),
					new CookieOptions { Expires = _globalAppSettings.DefaultCookieLifespan }
				);
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
