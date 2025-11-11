using Microsoft.AspNetCore.Identity;
using Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFAPP.Components {
	public partial class DetailsControl : Grid {
		static public event EventHandler ItemCreated = delegate { };
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private DataGrid _dataGrid = null!;
		private dynamic _dataType = null!;
		private IdentityRole<string>[] _permissiveRoles = null!;
		private bool _isSettingDataContext = false;
		private bool _isEditing = false;

		// Define input requirements
		// Key: Field name, Value: Human-readable name
		public Dictionary<Control, string> inputRequirements = new();

		public DetailsControl(AgendaDbContext context, UserManager<AgendaUser> userManager, DataGrid dataGrid, dynamic type, IdentityRole<string>[] permissiveRoles) {
			_context = context;
			_userManager = userManager;
			_dataGrid = dataGrid;
			_dataType = type;
			_permissiveRoles = permissiveRoles;
			InitializeComponent();
		}

		public void grDetails_InfoChanged(object? sender, EventArgs e) {
			if (_isSettingDataContext) {
				return;
			}

			// Show save button when all details are filled
			btnSave.IsEnabled = MainWindow.ValidateInputs(out _, in inputRequirements);
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			_isEditing = false;
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(_dataGrid.SelectedItem);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			btnAdd.IsEnabled = false;
			_isEditing = true;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				dynamic item = Convert.ChangeType(_dataGrid.SelectedItem, _dataType);
				dynamic? contextItem = _context.Find(_dataType, item.Id);

				if (contextItem == null) {
					throw new KeyNotFoundException(message: $"Kon de item van het type {_dataType} niet vinden in de database met de ID: {item.Id ?? ""}.");
				}

				contextItem.Deleted = DateTime.Now;
				_context.SaveChanges();

				// Reset the selected item and refresh the DataGrid
				_dataGrid.SelectedItem = CollectionView.NewItemPlaceholder;
				UpdateDataGrid();
			} catch (Exception ex) {
				MessageBox.Show(
					 $"Error: {ex.Message}\n",
					 "Fout details-scherm verwijdering",
					 MessageBoxButton.OK,
					 MessageBoxImage.Error
				);
				UpdateDataGrid();
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			spError.Children.Clear();

			// Validate inputs and show errors
			MainWindow.ValidateInputs(out List<Control> errors, in inputRequirements);

			foreach (Control field in inputRequirements.Keys) {
				field.ClearValue(Border.BorderBrushProperty);

				// If invalid, add error
				if (errors.Contains(field)) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"{inputRequirements[field]} is vereist" });
				}
			}

			// If there are errors, return early
			if (spError.Children.Count > 0) {
				return;
			}

			try {
				dynamic item;
				if (_isEditing) {
					// Get the DbSet of the provided type
					var set = _context.Set(_dataType);
					IEnumerable<dynamic> query = set.AsQueryable().Cast<dynamic>();

					// Get the specifc instance that is selected assuming it has an ID property
					item = query.First(item => item.Id == ((dynamic) DataContext).Id);
					SetDynamicProperties(item);

					// Save to database
					_context.Appointments.Update(item);
					_context.SaveChanges();
					_isEditing = false;
				} else {
					// Attempt to create a new item instance
					item = Activator.CreateInstance(_dataType);
					item.AgendaUserId = App.User.Id;
					SetDynamicProperties(item);

					// Save to database and notify subscribers
					_context.Appointments.Add(item);
					_context.SaveChanges();
					ItemCreated?.Invoke(typeof(DetailsControl), new EventArgs());

					// Show success message
					MessageBox.Show("Afspraak succesvol aangemaakt.");
				}
				grDetailsInputs.Visibility = Visibility.Collapsed;

				// Refresh the DataGrid and select the item
				UpdateDataGrid();
				_dataGrid.SelectedItem = item;
			} catch (Exception ex) {
				MessageBox.Show(
					$"Error: {ex.Message}\n" +
					"U zult andere waarden moeten proberen of contact opnemen met de support.",
					"Fout details-scherm afspraak aanmaak",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
			}
		}

		private void SetDynamicProperties(dynamic item) {
			bool hasError = false;

			foreach (Control field in inputRequirements.Keys) {
				string propertyName = field.Name.Length > 2 ? field.Name.Substring(2) : field.Name;
				dynamic? value = null;

				switch (field) {
					case TextBox textBox:
						value = textBox.Text;
						break;
					case ComboBox comboBox:
						value = comboBox.SelectedItem;
						break;
					case DatePicker datePicker:
						value = datePicker.SelectedDate;
						break;
					case PasswordBox passwordBox:
						value = passwordBox.Password;
						break;
				}

				// Check for null or an empty value
				if (value == null || (value is string s && string.IsNullOrWhiteSpace(s))) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"'{inputRequirements[field]}' is vereist" });
					hasError = true;
					continue;
				}

				// Check if property exists
				dynamic? prop = item.GetType().GetProperty(propertyName);
				if (prop == null) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"Eigrnschap '{propertyName}' bestaat niet." });
					hasError = true;
					continue;
				}

				// Check if property is writable
				if (!prop.CanWrite) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"Eigrnschap '{propertyName}' is niet schrijfbaar." });
					hasError = true;
					continue;
				}

				// Finally, attempt to set the property value
				try {
					prop.SetValue(item, value);
				} catch (Exception ex) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"Fout bij instellen van de eigenschap '{propertyName}': {ex.Message}" });
					hasError = true;
				}
			}

			if (hasError) {
				throw new InvalidOperationException("Niet alle velden zijn correct ingevuld.");
			}
		}

		public void UpdateDataGrid(Func<dynamic, bool>? filterOptions = null) {
			dynamic modelType = _context.Model.FindEntityType(_dataType.ToString());

			/// <summary>
			/// Dictionary implementaiton is mostly a copy and paste by Moho: <see href="https://stackoverflow.com/a/52638774"/>
			/// </summary>
			Dictionary<string, Func<AgendaDbContext, IEnumerable<dynamic>>> dbSetDictionary = new() {
				 { "Models.AgendaUser", ( AgendaDbContext context ) => context.Set<AgendaUser>() },
				 { "Models.Vehicle", ( AgendaDbContext context ) => context.Set<Vehicle>() },
				 { "Models.Appointment", ( AgendaDbContext context ) => context.Set<Appointment>() }
			};

			Func<AgendaDbContext, IEnumerable<dynamic>> dbSet = dbSetDictionary[modelType.Name];
			IEnumerable<dynamic> query = dbSet.Invoke(_context);

			// Always filter out deleted items
			query = query.Where(item => item.Deleted >= DateTime.Now);

			// Apply custom filter if provided
			if (filterOptions != null) {
				query = query.Where(filterOptions);
			}

			List<dynamic> items = query.OrderBy(item => item.Date).ToList();
			_dataGrid.ItemsSource = items;
			UpdateUIButtons();
		}

		public void UpdateUIButtons() {
			if (App.User == null) {
				return;
			}

			// btnAdd visible when a real user is logged in
			btnAdd.IsEnabled = (App.User != AgendaUser.Dummy);

			// If no selection, nothing more to do
			if (_dataGrid.SelectedItem == CollectionView.NewItemPlaceholder || _dataGrid.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
				SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);
				return;
			}

			// Only allow edit/delete to the logged-in user if they have the correct permissions
			List<string> roleList = _context.UserRoles
				.Where(ur => ur.UserId == App.User.Id)
				.Select(ur => ur.RoleId)
				.ToList();
			if (roleList.Count > 0 && _permissiveRoles.Any(ur => roleList.Contains(ur.Id))) {
				btnEdit.IsEnabled = true;
				btnDelete.IsEnabled = true;
			}
			btnSave.IsEnabled = false;

			// Update the grDetails grid to use the currently selected item data
			SetDataContextWithoutTriggeringEvent(_dataGrid.SelectedItem);
		}

		private void SetDataContextWithoutTriggeringEvent(object item) {
			_isSettingDataContext = true;
			grDetailsInputs.DataContext = item;
			_isSettingDataContext = false;
		}
	}
}
