using System;
using System.Collections.Generic;
using System.Reflection;

namespace PoorMans.DI
{
	// ServiceLifetime determines if we need to create a new instance or use an old one
	public enum ServiceLifetime
	{
		Singleton, Scoped, Transient
	}

	// ServiceDescriptor contains information about single service
	public class ServiceDescriptor
	{
		public Type Service { get; private set; }
		public Type Implementation { get; private set; }
		public ServiceLifetime LifeTime { get; private set; }
		internal object Instance { get; set; }

		public ServiceDescriptor(Type service, Type implementation, ServiceLifetime lifetime)
		{
			Service = service;
			Implementation = implementation;
			LifeTime = lifetime;
			Instance = null;
		}
	}

	// ServiceCollection stores service list and allows us to create Service Provider needed to run services
	public class ServiceCollection : IEnumerable<ServiceDescriptor>
	{
		private Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();

		// Add service as "singleton". There are will be only 1 instance of (shared) service
		public ServiceCollection AddSingleton<TService, TImplementation>()
		{
			_services.Add(typeof(TService),
				new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
			return this;
		}

		// Add service as "scoped". New instance will be created for each scope
		public ServiceCollection AddScoped<TService, TImplementation>()
		{
			_services.Add(typeof(TService),
				new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
			return this;
		}

		// Add service as "transient". New instance will be created for each request
		public ServiceCollection AddTransient<TService, TImplementation>()
		{
			_services.Add(typeof(TService),
				new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
			return this;
		}

		// Creates ServiceProvider object used to instantiate services
		public ServiceProvider BuildServiceProvider()
		{
			return new ServiceProvider(_services);
		}

		// IEnumerable implementation to iterate the collection of services
		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return _services.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _services.GetEnumerator();
		}
	}

	// ServiceProvider object allows to instantiate services defined in ServiceCollection
	public class ServiceProvider
	{
		private Dictionary<Type, ServiceDescriptor> _services;

		// The constructor should not be called directly
		internal ServiceProvider(Dictionary<Type, ServiceDescriptor> services)
		{
			_services = services;
		}

		// GetService instantiates requested service or returns an existing object
		// depending on a service's lifetime
		public TService GetService<TService>()
		{
			// Is it a known type?
			ServiceDescriptor desc;
			if (!_services.TryGetValue(typeof(TService), out desc))
			{
				return default(TService);
			}

			// Create instance and add it to a singleton storage
			var o = Instantiate(desc);

			return (TService)o;
		}

		// Returns appropriate constructor
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

		// Instantiate creates a new instance of service if needed
		// and recursively resolves dependencies
		private object Instantiate(ServiceDescriptor service)
		{
			// Check if we already created an object of this type
			if (service.LifeTime == ServiceLifetime.Singleton && service.Instance != null)
			{
				return service.Instance;
			}

			var con = FindConstructor(service.Implementation);

			var par = con.GetParameters();

			var args = new List<object>();

			// Create constructor arguments
			// and instantiate known types
			foreach (var p in par)
			{
				ServiceDescriptor desc;
				if (!_services.TryGetValue(p.ParameterType, out desc))
				{
					args.Add(Activator.CreateInstance(p.ParameterType));
				}
				else
				{
					args.Add(Instantiate(desc));
				}
			}

			// Create a new object of service's type
			var inst = Activator.CreateInstance(service.Implementation, args.ToArray());

			if (service.LifeTime == ServiceLifetime.Singleton)
			{
				service.Instance = inst;
			}

			// Check if we need property injection
			foreach (var prop in inst.GetType().GetProperties())
			{
				ServiceDescriptor desc;
				if (_services.TryGetValue(prop.PropertyType, out desc))
				{
					// Don't touch initialized properties
					if (prop.GetValue(inst, null) == null)
					{
						prop.SetValue(inst, Instantiate(desc), null);
					}
				}
			}

			return inst;
		}
	}
}
