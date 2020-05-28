using SereneApi.Enums;
using SereneApi.Interfaces;
using System;
using System.Collections.Generic;

namespace SereneApi.Types
{
    public class DependencyCollection : IDependencyCollection, IDisposable
    {
        private readonly Dictionary<Type, IDependency> _dependencies = new Dictionary<Type, IDependency>();

        /// <summary>
        /// Adds a new <see cref="Dependency{TDependency}"/> or overrides an existing <see cref="Dependency{TDependency}"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <param name="dependencyInstance"></param>
        /// <param name="binding"></param>
        public void AddDependency<TDependency>(TDependency dependencyInstance, DependencyBinding binding = DependencyBinding.Bound)
        {
            Dependency<TDependency> dependency = new Dependency<TDependency>(dependencyInstance, binding);

            if (_dependencies.ContainsKey(dependency.Type))
            {
                _dependencies[dependency.Type] = dependency;
            }
            else
            {
                _dependencies.Add(dependency.Type, dependency);
            }
        }

        public bool TryGetDependency<TDependency>(out TDependency dependencyInstance)
        {
            if (_dependencies.TryGetValue(typeof(TDependency), out IDependency dependency))
            {
                dependencyInstance = dependency.GetInstance<TDependency>();

                return true;
            }

            dependencyInstance = default;

            return false;
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (IDependency dependency in _dependencies.Values)
                {
                    if (dependency is IDisposable disposableDependency)
                    {
                        disposableDependency.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
