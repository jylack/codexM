// 서비스 인스턴스를 타입 기반으로 등록/조회하는 간단한 DI 레지스트리입니다.
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
