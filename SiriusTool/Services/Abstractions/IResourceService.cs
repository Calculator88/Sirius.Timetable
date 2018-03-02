namespace SiriusTool.Services.Abstractions
{
	public interface IResourceService
	{
		string GetDialogTitleString();
		string GetDialogNoInternetString();
		string GetDialogCacheIsStaleString();
	}
}