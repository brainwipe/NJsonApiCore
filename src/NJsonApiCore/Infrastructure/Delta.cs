using NJsonApi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJsonApi.Conventions.Impl;

namespace NJsonApi.Infrastructure
{
    public class Delta<T> : IDelta<T> where T : new()
    {
        private Dictionary<string, Action<T, object>> _currentTypeSetters;
        private Dictionary<string, Action<T, object>> _typeSettersTemplates;

        private Dictionary<string, CollectionInfo<T>> _currentCollectionInfos;
        private Dictionary<string, CollectionInfo<T>> _collectionInfoTemplates;

        public Dictionary<string, object> ObjectPropertyValues { get; set; }
        public Dictionary<string, ICollectionDelta> CollectionDeltas { get; set; }
        public IMetaData TopLevelMetaData { get; set; }
        public IMetaData ObjectMetaData { get; set; }
        private bool _scanned;

        public Delta()
        {
            ObjectPropertyValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CollectionDeltas = new Dictionary<string, ICollectionDelta>();
            TopLevelMetaData = null;
            ObjectMetaData = null; 
        }

        public void Scan()
        {
            if (_typeSettersTemplates == null)
            {
                _typeSettersTemplates = ScanForProperties();
            }
            if (_collectionInfoTemplates == null)
            {
                _collectionInfoTemplates = ScanForCollections();
            }

            _currentTypeSetters = _typeSettersTemplates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            _currentCollectionInfos = _collectionInfoTemplates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            _scanned = true;
        }

        public void FilterOut<TProperty>(params Expression<Func<T, TProperty>>[] filter)
        {
            ThrowExceptionIfNotScanned();
            foreach (var f in filter)
            {
                var propertyName = f.GetPropertyInfo().Name;
                if (_currentTypeSetters.ContainsKey(propertyName))
                _currentTypeSetters.Remove(propertyName);
                _currentCollectionInfos.Remove(propertyName);
            }
        }

        public void SetValue<TProperty>(Expression<Func<T, TProperty>> property, object value)
        {
            var propertyInfo = property.GetPropertyInfo();
            ObjectPropertyValues[propertyInfo.Name] = value;
        }

        public TProperty GetValue<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = property.GetPropertyInfo();
            object val;
            ObjectPropertyValues.TryGetValue(propertyInfo.Name, out val);
            return (TProperty)val;
        }

        public static string ToProperCase(string the_string)
        {
            return the_string.Substring(0, 1).ToUpper() + the_string.Substring(1);
        }

        public void ApplySimpleProperties(T inputObject)
        {
            ThrowExceptionIfNotScanned();
            if (ObjectPropertyValues == null) return;
            foreach (var objectPropertyNameValue in ObjectPropertyValues)
            {
                Action<T, object> setter;

                _currentTypeSetters.TryGetValue(ToProperCase(objectPropertyNameValue.Key), out setter);
                if (setter != null)
                    setter(inputObject, objectPropertyNameValue.Value);
            }
        }

        public void ApplyCollections(T inputObject)
        {
            ThrowExceptionIfNotScanned();
            if (ObjectPropertyValues == null) return;
            foreach (var colDelta in CollectionDeltas)
            {
                CollectionInfo<T> info;
                _currentCollectionInfos.TryGetValue(ToProperCase(colDelta.Key), out info);
                if (info != null)
                {
                    var existingCollection = info.Getter(inputObject);
                    if (existingCollection == null)
                    {
                        existingCollection = Activator.CreateInstance(info.CollectionType) as ICollection;
                        info.Setter(inputObject, existingCollection);
                    }

                    colDelta.Value.Apply(existingCollection);
                }
            }
        }

        public ICollectionDelta<TElement> Collection<TElement>(Expression<Func<T, ICollection<TElement>>> collectionProperty)
        {
            ICollectionDelta delta;
            CollectionDeltas.TryGetValue(collectionProperty.GetPropertyInfo().Name, out delta);
            return delta as ICollectionDelta<TElement>;
        }

        public T ToObject()
        {
            var t = new T();
            ApplySimpleProperties(t);
            ApplyCollections(t);
            return t;
        }

        private Dictionary<string, Action<T, object>> ScanForProperties()
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => ObjectPropertyValues.ContainsKey(pi.Name))
                .ToDictionary(pi => pi.Name, pi => pi.ToCompiledSetterAction<T, object>());
        }

        private Dictionary<string, CollectionInfo<T>> ScanForCollections()
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => CollectionDeltas.ContainsKey(pi.Name) && 
                (typeof(ICollection).IsAssignableFrom(pi.PropertyType)))
                .ToDictionary(pi => pi.Name, pi => new CollectionInfo<T>
                {
                    Getter = pi.ToCompiledGetterFunc<T, ICollection>(),
                    Setter = pi.ToCompiledSetterAction<T, ICollection>(),
                    CollectionType = pi.PropertyType,
                });
        }

        private class CollectionInfo<TOwner>
        {
            public Type CollectionType { get; set; }
            public Func<TOwner, ICollection> Getter { get; set; }
            public Action<TOwner, ICollection> Setter { get; set; }
        }

        private void ThrowExceptionIfNotScanned()
        {
            if (!_scanned) throw new Exception("Scan must be called before this method");
        }
    }
}