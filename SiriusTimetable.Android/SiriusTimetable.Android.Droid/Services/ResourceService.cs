using System;
using Android.Content.Res;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class ResourceService : IResourceService
	{
		private readonly Resources _resources;

		public ResourceService(Resources resources)
		{
			_resources = resources;
		}

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