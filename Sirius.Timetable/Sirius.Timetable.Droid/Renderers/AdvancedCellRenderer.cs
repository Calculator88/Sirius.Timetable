using System.ComponentModel;
using Android.Content;
using Android.Views;
using Sirius.Timetable.Controls;
using Sirius.Timetable.Droid.Controls;
using Sirius.Timetable.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;
using Android.Text;

[assembly: ExportRenderer(typeof(AdvancedCell), typeof(AdvancedCellRenderer))]
namespace Sirius.Timetable.Droid.Renderers
{
	public class AdvancedCellRenderer : ViewCellRenderer
	{
		private AdvancedCellControler _cell;
		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var advancedCell = (AdvancedCell) item;
			_cell = convertView as AdvancedCellControler;
			if (_cell == null)
			{
				_cell = new AdvancedCellControler(context, advancedCell);
			}
			else
			{
				_cell.AdvancedCell.PropertyChanged -= AdvancedCellOnPropertyChnaged;
			}
			advancedCell.PropertyChanged += AdvancedCellOnPropertyChnaged;

			_cell.UpdateCell(advancedCell);

			return _cell;
		}

		private void AdvancedCellOnPropertyChnaged(object sender, PropertyChangedEventArgs e)
		{
			var advancedCell = (AdvancedCell) sender;
			if (e.PropertyName == AdvancedCell.StartProperty.PropertyName) //Start
				_cell.StartTextView.Text = advancedCell.Start;
			else if (e.PropertyName == AdvancedCell.EndProperty.PropertyName) //End
				_cell.EndTextView.Text = advancedCell.End;
			else if (e.PropertyName == AdvancedCell.BusToProperty.PropertyName) //BusTo
				_cell.BusToTextView.Text = advancedCell.BusTo;
			else if (e.PropertyName == AdvancedCell.BusFromProperty.PropertyName) //BusFrom
				_cell.BusFromTextView.Text = advancedCell.BusFrom;
			else if (e.PropertyName == AdvancedCell.TitleProperty.PropertyName) //Title
				_cell.TitleTextView.Text = advancedCell.Title;
			else if (e.PropertyName == AdvancedCell.PlaceProperty.PropertyName) //Place
				_cell.PlaceTextView.Text = advancedCell.Place;
			else if (e.PropertyName == AdvancedCell.MainTextSizeProperty.PropertyName) //MainTextSize
				_cell.StartTextView.TextSize = _cell.EndTextView.TextSize = _cell.DashTextView.TextSize = _cell.TitleTextView.TextSize = advancedCell.MainTextSize;
			else if (e.PropertyName == AdvancedCell.DetailTextSizeProperty.PropertyName) //DetailTextSize
				_cell.BusToTextView.TextSize =
					_cell.BusFromTextView.TextSize =
						_cell.PlaceTextView.TextSize = advancedCell.DetailTextSize;
			else if (e.PropertyName == AdvancedCell.IsSelectedProperty.PropertyName) // IsSelected
			{
				_cell.Details.Visibility = advancedCell.IsSelected ? ViewStates.Visible : ViewStates.Gone;
				_cell.TitleTextView.SetSingleLine(!advancedCell.IsSelected);
				_cell.TitleTextView.Ellipsize = advancedCell.IsSelected ? null : TextUtils.TruncateAt.Marquee;
			}
			else if (e.PropertyName == AdvancedCell.IsBusProperty.PropertyName) // IsBus
				_cell.Bus.Visibility = advancedCell.IsBus ? ViewStates.Visible : ViewStates.Gone;
			else if (e.PropertyName == AdvancedCell.IsPlaceProperty.PropertyName) //IsPlace
				_cell.PlaceTextView.Visibility = advancedCell.IsPlace ? ViewStates.Visible : ViewStates.Gone;
			else if (e.PropertyName == AdvancedCell.BackgroundColorProperty.PropertyName) // BackgroundColor
				_cell.BackLayout.SetBackgroundColor(advancedCell.BackgroundColor.ToAndroid());
		}
	}
}