using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestRegx
{
	class Program
	{
		static void Main()
		{
			var regx = new Regex(@"\b([НЛИС])(\d+)\b");
			var str = Console.ReadLine();
			var match = regx.Match(str);
		
			foreach(var item in match.Groups)
			{
				Console.WriteLine(item);
			}
		}
	}
}