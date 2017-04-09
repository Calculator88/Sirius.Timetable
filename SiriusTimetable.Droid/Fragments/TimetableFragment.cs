using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Droid.Helpers;

namespace SiriusTimetable.Droid.Fragments
{
	public class TimetableFragment : Fragment, RecyclerViewAdapter.IItemClickListener, RecyclerViewAdapter.IItemLongClickListener
	{
		#region Private fields

		private RecyclerView _recyclerView;
		private LinearLayoutManager _manager;
		private RecyclerViewAdapter _adapter;
		private IOnItemSelected _listener;

		#endregion

		#region Fragment lifecycle

		public override void OnAttach(Context context)
		{
			base.OnAttach(context);
			_listener = Activity as IOnItemSelected;
			if (_listener == null) throw new Exception("Activity must be inherited from TimetableFragment.IOnItemSelected interface");
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.TimetableFragment, container, false);
			_recyclerView = v.FindViewById<RecyclerView>(Resource.Id.recycler);

			_manager = new LinearLayoutManager(Activity);
			_recyclerView.SetAdapter(null);
			_recyclerView.SetLayoutManager(_manager);
			_recyclerView.AddItemDecoration(new DividerItemDecoration(_recyclerView.Context, _manager.Orientation));

			var vm = ServiceLocator.GetService<TimetableViewModel>();
			if (vm.Timetable != null)
				SetItems(vm.Timetable.ToList());

			return v;
		}

		#endregion

		#region Listeners

		public void ItemClick(TimetableItem item)
		{
			_listener.ItemSelected(item);
		}
		public void ItemLongClick(TimetableItem item)
		{
			_listener.ItemSelected(item);
		}

		#endregion

		#region Interaction interfaces

		public interface IOnItemSelected
		{
			void ItemSelected(TimetableItem item);
		}

		#endregion

		#region Public methods

		public void SetItems(List<TimetableItem> items)
		{
			_adapter = new RecyclerViewAdapter(items, this, this);
			_recyclerView.SetAdapter(_adapter);
		}

		#endregion
	}
}