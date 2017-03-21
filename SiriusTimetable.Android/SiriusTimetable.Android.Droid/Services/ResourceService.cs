using System;
using Android.Content.Res;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class ResourceService : IResourceService
	{
		public ResourceService(Resources resources)
		{
			_resources = resources;
		}

		private readonly Resources _resources;
		public String GetDialogTitleString()
		{
			return _resources.GetString(Resource.String.AlertTitle);
		}

		public String GetDialogNoInternetString()
		{
			return _resources.GetString(Resource.String.AlertNoInternetMessage);
		}

		public String GetDialogCacheIsStaleString()
		{
			return _resources.GetString(Resource.String.AlertStaleCacheMessage);
		}
	}
}