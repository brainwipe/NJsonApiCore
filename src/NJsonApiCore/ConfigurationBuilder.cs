using NJsonApi.Conventions;
using NJsonApi.Conventions.Impl;
using NJsonApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NJsonApi
{
    public class ConfigurationBuilder
    {
        public readonly Dictionary<Type, IResourceConfigurationBuilder> ResourceConfigurationsByType = new Dictionary<Type, IResourceConfigurationBuilder>();

        private readonly Stack<IConvention> conventions = new Stack<IConvention>();

        public ConfigurationBuilder()
        {
            //add the default conventions
            WithConvention(new PluralizedCamelCaseTypeConvention());
            WithConvention(new CamelCaseLinkNameConvention());
            WithConvention(new SimpleLinkedIdConvention());
            WithConvention(new DefaultPropertyScanningConvention());
        }

        public ConfigurationBuilder WithConvention<T>(T convention) where T : class, IConvention
        {
            conventions.Push(convention);
            return this;
        }

        public T GetConvention<T>() where T : class, IConvention
        {
            var firstMatchingConvention = conventions
                .OfType<T>()
                .FirstOrDefault();
            if (firstMatchingConvention == null)
                throw new InvalidOperationException($"No convention found for type {typeof(T).Name}");
            return firstMatchingConvention;
        }

        public ResourceConfigurationBuilder<TResource, TController> Resource<TResource, TController>()
        {
            var resource = typeof(TResource);
            var controller = typeof(TResource);

            if (DoesControllerHaveMoreThanOneGetForThisResource(controller, resource))
            {
                throw new InvalidOperationException($"The controller being registered ({controller.FullName}) can only have one GET method with single id parameter for this resource type.");
            }

            if (DoesModelHaveReservedWordsRecursive(resource))
            {
                throw new InvalidOperationException($"The resource being registered ({resource.FullName}) contains properties that are reserved words by JsonApi (Relationships, Links).");
            }

            if (!AssertModelHasIdProperty(resource))
            {
                throw new InvalidOperationException($"The resource being registered ({resource.FullName}) must contain an Id property. It can be of any value type.");
            }

            if (!ResourceConfigurationsByType.ContainsKey(resource))
            {
                var newResourceConfiguration = new ResourceConfigurationBuilder<TResource, TController>(this);
                ResourceConfigurationsByType[resource] = newResourceConfiguration;
                return newResourceConfiguration;
            }
            else
            {
                return ResourceConfigurationsByType[resource] as ResourceConfigurationBuilder<TResource, TController>;
            }
        }

        private bool DoesControllerHaveMoreThanOneGetForThisResource(Type resource, Type controller)
        {
            return controller.GetMethods().Any(m =>
                m.ReturnType == resource
                &&
                m.GetParameters().Any(p => p.Name == "id")
                &&
                m.GetParameters().Count() == 1);
        }

        public Configuration Build()
        {
            var configuration = new Configuration();
            var propertyScanningConvention = GetConvention<IPropertyScanningConvention>();

            // Each link needs to be wired to full metadata once all resources are registered
            foreach (var resourceConfiguration in ResourceConfigurationsByType)
            {
                var links = resourceConfiguration.Value.BuiltResourceMapping.Relationships;
                for (int i = links.Count - 1; i >= 0; i--)
                {
                    var link = links[i];
                    IResourceConfigurationBuilder resourceConfigurationOutput;
                    if (!ResourceConfigurationsByType.TryGetValue(link.RelatedBaseType, out resourceConfigurationOutput))
                    {
                        if (propertyScanningConvention.ThrowOnUnmappedLinkedType)
                        {
                            throw new InvalidOperationException(
                                $"Type {link.ParentType.Name} was registered to have a linked resource {link.RelationshipName} of type {link.RelatedBaseType.Name} which was not registered. Register resource type {link.RelatedBaseType.Name} or disable serialization of that property.");
                        }
                        else
                            links.RemoveAt(i);
                    }
                    else
                        link.ResourceMapping = resourceConfigurationOutput.BuiltResourceMapping;
                }

                configuration.AddMapping(resourceConfiguration.Value.BuiltResourceMapping);
            }

            return configuration;
        }

        private bool AssertModelHasIdProperty(Type model)
        {
            foreach (var property in model.GetProperties())
            {
                if (property.Name == "Id" || property.Name == "id")
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoesModelHaveReservedWordsRecursive(Type model, List<Type> checkedTypes = null)
        {
            if (checkedTypes == null)
                checkedTypes = new List<Type>();

            if (checkedTypes.Contains(model))
            {
                return false;
            }
            else
            {
                checkedTypes.Add(model);
            }

            foreach (var property in model.GetProperties())
            {
                if (property.Name == "Relationships" || property.Name == "Links")
                {
                    return true;
                }

                var childTypesToScan = Reflection.FromWithinGeneric(property.PropertyType);

                foreach (var childType in childTypesToScan)
                {
                    if (childType.GetTypeInfo().IsClass)
                    {
                        return DoesModelHaveReservedWordsRecursive(childType, checkedTypes);
                    }
                }
            }
            return false;
        }
    }
}