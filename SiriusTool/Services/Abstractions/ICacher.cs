using System;
using System.IO;

namespace SiriusTool.Services.Abstractions
{
	public interface ICacher
	{
	    FileInfo GetInfo(string fileName);

	    FileInfo GetInfoForTeam(string team);
	}
}