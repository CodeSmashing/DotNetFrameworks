using Models;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class ToDoControl : UserControl {
		private readonly AgendaDbContext _context;

		public ToDoControl(AgendaDbContext context) {
			_context = context;
			InitializeComponent();
		}
	}
}
