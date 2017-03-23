namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface IResourceService
	{
		string GetDialogTitleString();
		string GetDialogNoInternetString();
		string GetDialogCacheIsStaleString();
	}
}