using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFAPP
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:WPFAPP"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:WPFAPP;assembly=WPFAPP"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:CustomControl1/>
	///
	/// </summary>
	public class VehicleSelector : Control
	{
		static VehicleSelector()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VehicleSelector), new FrameworkPropertyMetadata(typeof(VehicleSelector)));
		}

		public VehicleSelector()
		{
			SelectImageCommand = new RelayCommand(p =>
			{
				if (p is ImageSource img)
				{
					SelectedImage = img;
					// SelectedIndex will be computed from ItemsSource if possible
					UpdateSelectedIndexFromImage(img);
				}
			});
		}

		// ItemsSource: collection of ImageSource (e.g., BitmapImage)
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(
				nameof(ItemsSource),
				typeof(IEnumerable),
				typeof(VehicleSelector),
				new PropertyMetadata(null, OnItemsSourceChanged));

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctl = (VehicleSelector)d;
			// If SelectedImage exists, try to update SelectedIndex
			if (ctl.SelectedImage != null)
				ctl.UpdateSelectedIndexFromImage(ctl.SelectedImage);
		}

		// SelectedImage: currently chosen image
		public static readonly DependencyProperty SelectedImageProperty =
			DependencyProperty.Register(
				nameof(SelectedImage),
				typeof(ImageSource),
				typeof(VehicleSelector),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedImageChanged));

		public ImageSource SelectedImage
		{
			get => (ImageSource)GetValue(SelectedImageProperty);
			set => SetValue(SelectedImageProperty, value);
		}

		private static void OnSelectedImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctl = (VehicleSelector)d;
			var img = e.NewValue as ImageSource;
			ctl.UpdateSelectedIndexFromImage(img);
			ctl.RaiseSelectedImageChanged();
		}

		// SelectedIndex: index of selection in ItemsSource if ItemsSource is indexable
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register(
				nameof(SelectedIndex),
				typeof(int),
				typeof(VehicleSelector),
				new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

		public int SelectedIndex
		{
			get => (int)GetValue(SelectedIndexProperty);
			set => SetValue(SelectedIndexProperty, value);
		}

		private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctl = (VehicleSelector)d;
			var idx = (int)e.NewValue;
			ctl.UpdateSelectedImageFromIndex(idx);
			ctl.RaiseSelectedIndexChanged();
		}

		// ICommand to be used in the template (bind to Image click)
		public ICommand SelectImageCommand { get; }

		private void UpdateSelectedIndexFromImage(ImageSource img)
		{
			if (ItemsSource == null)
			{
				SelectedIndex = -1;
				return;
			}

			int index = 0;
			bool found = false;

			foreach (var item in ItemsSource)
			{
				if (item is ImageSource isrc)
				{
					if (Equals(isrc, img))
					{
						SelectedIndex = index;
						found = true;
						break;
					}
				}
				else if (item != null && item.Equals(img))
				{
					SelectedIndex = index;
					found = true;
					break;
				}
				index++;
			}

			if (!found)
				SelectedIndex = -1;
		}

		private void UpdateSelectedImageFromIndex(int idx)
		{
			if (ItemsSource == null || idx < 0)
			{
				SelectedImage = null;
				return;
			}

			int index = 0;
			foreach (var item in ItemsSource)
			{
				if (index == idx)
				{
					if (item is ImageSource isrc)
						SelectedImage = isrc;
					else
						SelectedImage = null;
					return;
				}
				index++;
			}

			SelectedImage = null;
		}

		// Optional events
		public event RoutedEventHandler SelectedImageChanged;
		protected virtual void RaiseSelectedImageChanged()
		{
			SelectedImageChanged?.Invoke(this, new RoutedEventArgs());
		}

		public event RoutedEventHandler SelectedIndexChanged;
		protected virtual void RaiseSelectedIndexChanged()
		{
			SelectedIndexChanged?.Invoke(this, new RoutedEventArgs());
		}

		// Simple RelayCommand
		private class RelayCommand : ICommand
		{
			private readonly Action<object?> _execute;
			private readonly Func<object?, bool>? _canExecute;

			public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
			{
				_execute = execute ?? throw new ArgumentNullException(nameof(execute));
				_canExecute = canExecute;
			}

			public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

			public void Execute(object? parameter) => _execute(parameter);

			public event EventHandler? CanExecuteChanged
			{
				add => CommandManager.RequerySuggested += value;
				remove => CommandManager.RequerySuggested -= value;
			}
		}
	}
}
