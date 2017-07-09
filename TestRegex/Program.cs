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
			var str = "Start";
			foreach(var item in str)
			{
				Console.Write((int)item); Console.Write(' ');
			}
		}
	}
}
