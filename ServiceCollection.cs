using System;
using System.Collections.Generic;
using System.Reflection;

namespace PoorMans.DI
{
	public class ServiceCollection
	{
		private Dictionary<Type, Type> _singletonDictionary = new Dictionary<Type, Type>();
		private Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

		public ServiceCollection AddSingleton<TService, TImplementation>()
		{
			_singletonDictionary.Add(typeof(TService), typeof(TImplementation));
			return this;
		}

		public TService GetService<TService>()
		{
			// Is it a known type?
			Type type;
			if (!_singletonDictionary.TryGetValue(typeof(TService), out type))
			{
				return default(TService);
			}

			// Check if we already created an object of this type
			object o;
			if (_singletons.TryGetValue(typeof(TService), out o))
			{
				return (TService)o;
			}

			// Create instance and add it to a singleton storage
			o = Instantiate(type);
			_singletons[typeof(TService)] = o;

			return (TService)o;
		}

		private ConstructorInfo FindConstructor(Type type)
		{
			var cons = type.GetConstructors();

			foreach (var c in cons)
			{
				// Return first constructor with arguments
				// Hope it will be the one
				if (c.GetParameters().Length > 0)
				{
					return c;
				}
			}

			// Return the first one if nothing found
			if (cons.Length > 0)
				return cons[0];

			return null;
		}

		private object Instantiate(Type type)
		{
			var con = FindConstructor(type);

			var par = con.GetParameters();

			var args = new List<object>();

			// Create constructor arguments
			// and instantiate known types
			foreach (var p in par)
			{
				Type typ;
				if (!_singletonDictionary.TryGetValue(p.ParameterType, out typ))
				{
					args.Add(Activator.CreateInstance(p.ParameterType));
				}
				else
				{
					args.Add(Instantiate(typ));
				}
			}

			// Create object
			var inst = Activator.CreateInstance(type, args.ToArray());

			// Check if we need property injection
			foreach (var prop in inst.GetType().GetProperties())
			{
				Type typ;
				if (_singletonDictionary.TryGetValue(prop.PropertyType, out typ))
				{
					// Don't touch initialized properties
					if (prop.GetValue(inst, null) == null)
					{
						prop.SetValue(inst, Instantiate(typ), null);
					}
				}
			}

			return inst;
		}
	}
}
