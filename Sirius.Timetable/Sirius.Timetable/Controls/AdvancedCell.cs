using System;
using Xamarin.Forms;

namespace SiriusTimetable.Common.Controls
{
	public class AdvancedCell : ViewCell
	{

		public static readonly BindableProperty SelectedBackgroundColorProperty =
			BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(AdvancedCell), Color.Transparent,
				BindingMode.TwoWay);

		public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool),
			typeof(AdvancedCell), false, BindingMode.TwoWay);

		public static readonly BindableProperty StartProperty = BindableProperty.Create(nameof(Start), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty EndProperty = BindableProperty.Create(nameof(End), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty BusToProperty = BindableProperty.Create(nameof(BusTo), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty BusFromProperty = BindableProperty.Create(nameof(BusFrom), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty PlaceProperty = BindableProperty.Create(nameof(Place), typeof(string),
			typeof(AdvancedCell), "", BindingMode.TwoWay);

		public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor),
			typeof(Color), typeof(AdvancedCell), Color.Transparent, BindingMode.TwoWay);

		public static readonly BindableProperty IsBusProperty = BindableProperty.Create(nameof(IsBus), typeof(bool),
			typeof(AdvancedCell), false, BindingMode.TwoWay);

		public static readonly BindableProperty IsPlaceProperty = BindableProperty.Create(nameof(IsPlace), typeof(bool),
			typeof(AdvancedCell), false, BindingMode.TwoWay);

		public static readonly BindableProperty PhoneMainTextSizeProperty = BindableProperty.Create(
			nameof(PhoneMainTextSize), typeof(int), typeof(AdvancedCell), 14, BindingMode.TwoWay);

		public static readonly BindableProperty TabletMainTextSizeProperty =
			BindableProperty.Create(nameof(TabletMainTextSize), typeof(int), typeof(AdvancedCell), 16, BindingMode.TwoWay);

		public static readonly BindableProperty PhoneDetailTextSizeProperty =
			BindableProperty.Create(nameof(PhoneDetailTextSize), typeof(int), typeof(AdvancedCell), 12, BindingMode.TwoWay);

		public static readonly BindableProperty TabletDetailTextSizeProperty =
			BindableProperty.Create(nameof(TabletDetailTextSize), typeof(int), typeof(AdvancedCell), 14, BindingMode.TwoWay);

		public static readonly BindableProperty PhoneMaxLinesProperty = BindableProperty.Create(nameof(PhoneMaxLines),
			typeof(int), typeof(AdvancedCell), 2, BindingMode.TwoWay);

		public static readonly BindableProperty TabletMaxLinesProperty = BindableProperty.Create(nameof(TabletMaxLines),
			typeof(int), typeof(AdvancedCell), 1, BindingMode.TwoWay);

		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(ContentProperty),
			typeof(object), typeof(AdvancedCell), null, BindingMode.TwoWay);

		public event EventHandler NotifyButtonClicked;

		public void InvokeClick()
		{
			NotifyButtonClicked?.Invoke(Content, null);
		}

		public object Content
		{
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public Color SelectedBackgroundColor
		{
			get { return (Color) GetValue(SelectedBackgroundColorProperty); }
			set { SetValue(SelectedBackgroundColorProperty, value); }
		}

		public Color BackgroundColor
		{
			get { return (Color) GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public bool IsSelected
		{
			get { return (bool) GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public string Start
		{
			get { return (string) GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}

		public string End
		{
			get { return (string) GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public string BusTo
		{
			get { return (string) GetValue(BusToProperty); }
			set { SetValue(BusFromProperty, value); }
		}

		public string BusFrom
		{
			get { return (string) GetValue(BusFromProperty); }
			set { SetValue(BusFromProperty, value); }
		}

		public string Place
		{
			get { return (string) GetValue(PlaceProperty); }
			set { SetValue(PlaceProperty, value); }
		}

		public bool IsBus
		{
			get { return (bool) GetValue(IsBusProperty); }
			set { SetValue(IsBusProperty, value); }
		}

		public bool IsPlace
		{
			get { return (bool) GetValue(IsPlaceProperty); }
			set { SetValue(IsPlaceProperty, value); }
		}

		public int PhoneMainTextSize
		{
			get { return (int) GetValue(PhoneMainTextSizeProperty); }
			set { SetValue(PhoneMainTextSizeProperty, value); }
		}

		public int TabletMainTextSize
		{
			get { return (int) GetValue(TabletMainTextSizeProperty); }
			set { SetValue(TabletMainTextSizeProperty, value); }
		}

		public int PhoneDetailTextSize
		{
			get { return (int) GetValue(PhoneDetailTextSizeProperty); }
			set { SetValue(PhoneDetailTextSizeProperty, value); }
		}

		public int TabletDetailTextSize
		{
			get { return (int) GetValue(TabletDetailTextSizeProperty); }
			set { SetValue(TabletDetailTextSizeProperty, value); }
		}

		public int PhoneMaxLines
		{
			get { return (int) GetValue(PhoneMaxLinesProperty); }
			set { SetValue(PhoneMaxLinesProperty, value); }
		}

		public int TabletMaxLines
		{
			get { return (int) GetValue(TabletMaxLinesProperty); }
			set { SetValue(TabletMaxLinesProperty, value); }
		}
		
	}
}