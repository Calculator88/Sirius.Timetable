using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestRegex
{
	class Program
	{
		static void Main(string[] args)
		{
			var regx = new Regex(@"(\b([НИЛС])(\d+)\b)|(\b\w+[\w\s\W]*$)", RegexOptions.Compiled);
			var str = Console.ReadLine();
			var matches = regx.Matches(str);

			if(matches.Count == 1)
				Console.WriteLine(matches[0].Value);

			for (var i = 0; i < matches.Count - 1; ++i)
			{
				Console.WriteLine(matches[i].Value + ' ' + matches[matches.Count - 1].Value);
				Console.WriteLine(matches[i].Groups[2].Value + ' ' + matches[i].Groups[3].Value);
			}
		}
	}
}
