using System;
using SiriusTimetable.Common.Models;
using Xamarin.Forms;
using ListView = Xamarin.Forms.ListView;

namespace SiriusTimetable.Common.Views
{
	public partial class TimetablePage
	{
		public TimetablePage()
		{
			InitializeComponent();
		}

		private void ListViewOnActivityTapped(object sender, ItemTappedEventArgs e)
		{
			var item = (TimetableItem) e.Item;
			item.IsSelected = !item.IsSelected;
		}

		private void ListViewOnActivitySelected(object sender, SelectedItemChangedEventArgs e)
		{
			((ListView) sender).SelectedItem = null;
		}

		private void AdvancedCell_OnNotifyButtonClicked(Object sender, EventArgs e)
		{

		}
	}
}