namespace WPFAPP.CustomEventArgs {
	public class ItemCreatedEventArgs(dynamic item) : EventArgs {
		private readonly dynamic _item = item;

		public dynamic Item {
			get {
				return _item;
			}
		}
	}
}
