using System;
using System.Threading.Tasks;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface IDatePickerDialogService
	{
		Task<DateTime?> SelectedDate();
	}
}