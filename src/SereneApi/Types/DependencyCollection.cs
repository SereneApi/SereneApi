﻿using SereneApi.Enums;
using SereneApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SereneApi.Types
{
    /// <summary>
    /// A Collection of <see cref="IDependency"/>s.
    /// </summary>
    [DebuggerDisplay("Dependencies:{_dependencyTypeMap.Count}")]
    public class DependencyCollection : IDependencyCollection, IDisposable
    {
        private readonly Dictionary<Type, IDependency> _dependencyTypeMap;

        public DependencyCollection()
        {
            _dependencyTypeMap = new Dictionary<Type, IDependency>();
        }

        public DependencyCollection(Dictionary<Type, IDependency> dependencyTypeMap)
        {
            _dependencyTypeMap = dependencyTypeMap;
        }

        /// <summary>
        /// Adds a new <see cref="Dependency{TDependency}"/> or overrides an existing <see cref="Dependency{TDependency}"/>
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <param name="dependencyInstance"></param>
        /// <param name="binding"></param>
        public void AddDependency<TDependency>(TDependency dependencyInstance, Binding binding = Binding.Bound)
        {
            Dependency<TDependency> dependency = new Dependency<TDependency>(dependencyInstance, binding);

            if (_dependencyTypeMap.ContainsKey(dependency.Type))
            {
                _dependencyTypeMap[dependency.Type] = dependency;
            }
            else
            {
                _dependencyTypeMap.Add(dependency.Type, dependency);
            }
        }

        /// <inheritdoc cref="IDependencyCollection.GetDependency{TDependency}"/>
        public TDependency GetDependency<TDependency>()
        {
            if (!_dependencyTypeMap.TryGetValue(typeof(TDependency), out IDependency dependency))
            {
                throw new KeyNotFoundException($"Could not find the specified dependency {typeof(TDependency)}");
            }

            return dependency.GetInstance<TDependency>();
        }

        public List<TInterface> GetDependencies<TInterface>()
        {
            IEnumerable<TInterface> dependencies = _dependencyTypeMap.Where(m => m.Key.GetInterfaces().Contains(typeof(TInterface))).Select(m => (TInterface)m.Value.Instance);

            return dependencies.ToList();
        }

        /// <inheritdoc cref="IDependencyCollection.TryGetDependency{TDependency}"/>
        public bool TryGetDependency<TDependency>(out TDependency dependencyInstance)
        {
            if (_dependencyTypeMap.TryGetValue(typeof(TDependency), out IDependency dependency))
            {
                dependencyInstance = dependency.GetInstance<TDependency>();

                return true;
            }

            dependencyInstance = default;

            return false;
        }

        /// <inheritdoc cref="IDependencyCollection.HasDependency{TDependency}"/>
        public bool HasDependency<TDependency>()
        {
            return _dependencyTypeMap.ContainsKey(typeof(TDependency));
        }

        public object Clone()
        {
            Dictionary<Type, IDependency> newDependencyTypeMap = new Dictionary<Type, IDependency>(_dependencyTypeMap.Count, _dependencyTypeMap.Comparer);

            foreach (KeyValuePair<Type, IDependency> dependencyTypeMap in _dependencyTypeMap)
            {
                newDependencyTypeMap.Add(dependencyTypeMap.Key, (IDependency)dependencyTypeMap.Value.Clone());
            }

            return new DependencyCollection(newDependencyTypeMap);
        }

        #region IDisposable

        private volatile bool _disposed;

        /// <summary>
        /// Disposes the current instance of the <see cref="DependencyCollection"/>.
        /// </summary>
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
                foreach (IDependency dependency in _dependencyTypeMap.Values.ToList())
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
