using System;
using System.Collections.Generic;

namespace Project.Core
{
    public class ServiceRegistry
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public T Resolve<T>() where T : class
        {
            return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
        }
    }
}
