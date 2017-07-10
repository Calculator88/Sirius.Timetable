using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Droid.Helpers;
using System.ComponentModel;
using System.Linq;
using SiriusTimetable.Core.Services;
using Android.Support.V4.App;

namespace SiriusTimetable.Droid.Fragments
{
	public class TimetableFragment : Fragment, RecyclerViewAdapter.IItemClickListener, RecyclerViewAdapter.IItemLongClickListener
	{
		#region Private fields

		private RecyclerView _recyclerView;
		private LinearLayoutManager _manager;
		private RecyclerViewAdapter _adapter;
		private IOnItemSelected _listener;
		private TimetableViewModel _viewModel;

		#endregion

		#region Fragment lifecycle

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as IOnItemSelected;
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.TimetableFragment, container, false);
			_recyclerView = v.FindViewById<RecyclerView>(Resource.Id.recycler);

			_manager = new LinearLayoutManager(Activity);
			_recyclerView.SetAdapter(null);
			_recyclerView.SetLayoutManager(_manager);
			_recyclerView.AddItemDecoration(new DividerItemDecoration(_recyclerView.Context, _manager.Orientation));

			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += _viewModelOnPropertyChanged;
			UpdateVMLinks();

			return v;
		}
		public override void OnDestroy()
		{
			_viewModel.PropertyChanged -= _viewModelOnPropertyChanged;
			base.OnDestroy();
		}

		#endregion

		#region Listeners

		public void ItemClick(TimetableItem item)
		{
			_listener?.ItemSelected(item);
		}
		public void ItemLongClick(TimetableItem item)
		{
			_listener?.ItemSelected(item);
		}

		#endregion

		#region Private methods

		private void _viewModelOnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			var propName = e.PropertyName;
			switch(propName)
			{
				case nameof(_viewModel.Timetable):
					VMOnTimetableChanged();
					break;
				default:
					break;
			}
		}

		private void VMOnTimetableChanged()
		{
			SetItems(_viewModel.Timetable.ToList());
		}

		private void UpdateVMLinks()
		{
			VMOnTimetableChanged();
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

		#region Public fields

		public static readonly string LoadFromVMTag =  "LOADFROMVM";

		#endregion
	}
}