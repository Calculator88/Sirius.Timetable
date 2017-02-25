﻿using System;
using System.Collections.Generic;

namespace Sirius.Timetable.Core.Services
{
	public static class ServiceLocator
	{
		private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

		public static void RegisterService<T>(T instance)
		{
			Services[typeof(T)] = instance;
		}

		public static T GetService<T>()
		{
			return (T) Services[typeof(T)];
		}
	}
}