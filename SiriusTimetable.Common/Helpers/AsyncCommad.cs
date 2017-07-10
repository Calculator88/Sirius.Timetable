using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiriusTimetable.Common.Helpers
{
	public class AsyncCommand : ObservableObject, ICommand
	{
		private readonly Func<object, Task> _asyncExecutor;
		private bool _canExecute;

		public AsyncCommand(Func<object, Task> asyncExecutor)
		{
			_asyncExecutor = asyncExecutor;
		}

		public bool IsExecuting
		{
			get { return _canExecute; }
			set
			{
				SetProperty(ref _canExecute, value);
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool CanExecute(object parameter)
		{
			return !IsExecuting;
		}

		public async void Execute(object parameter)
		{
			IsExecuting = true;
			await _asyncExecutor(parameter);
			IsExecuting = false;
		}

		public event EventHandler CanExecuteChanged;
	}
}