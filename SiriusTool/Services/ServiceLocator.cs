using System;
using System.Collections.Generic;

namespace SiriusTool.Services
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
			if (!Services.ContainsKey(typeof(T)))
				throw new Exception($"{typeof(T)} is not registered");
			return (T) Services[typeof(T)];
		}
	}
}