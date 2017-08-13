using Android.Content;
using Android.Util;

namespace SiriusTimetable.Droid.Helpers
{
	internal class DipConverter
	{
		private readonly Context _context;
		public DipConverter(Context context)
		{
			_context = context;
		}

		public int ToDip(int pixels)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, pixels, _context.Resources.DisplayMetrics);
		}
	}
}