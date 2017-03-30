using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestControllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestSingleClass
    {
        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            Assert.NotNull(result.Data);
            var transformedObject = result.Data as SingleResource;
            Assert.NotNull(transformedObject);
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Id, objectToTransform.Id.ToString());
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Attributes["someValue"], objectToTransform.SomeValue);
            Assert.Equal(transformedObject.Attributes["date"], objectToTransform.DateTime);
            Assert.Equal(transformedObject.Attributes.Count, 2);
        }

        [Fact]
        public void Creates_CompondDocument_for_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Type, "sampleClasses");
        }

        [Fact]
        public void Creates_CompondDocument_for_single_derived_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateDerivedObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Type, "derivedClasses");
        }

        [Fact]
        public void Creates_CompondDocument_for_single_class_with_metadata_and_propertly_map_metadata()
        {
            // Arrange
            var context = CreateContext();
            SampleClassWithMetadata objectToTransform = CreateObjectWithMetadataToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal("value1", transformedObject.MetaData["meta1"]);
            Assert.Equal("value2", transformedObject.MetaData["meta2"]);
        }

        [Fact]
        public void Creates_CompondDocument_for_single_class_with_nometadata_and_propertly_map_nometadata()
        {
            // Arrange
            var context = CreateContext();
            SampleClass objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transformer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Null(transformedObject.MetaData);
        }

        private static SampleClass CreateObjectToTransform()
        {
            return new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped"
            };
        }

        private static DerivedClass CreateDerivedObjectToTransform()
        {
            return new DerivedClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped",
                DerivedProperty = "Derived value"
            };
        }

        private static SampleClassWithMetadata CreateObjectWithMetadataToTransform()
        {
            var o = new SampleClassWithMetadata
            {
                Id = 1,
                SomeValue = "Somevalue text test string"
            };
            o.GetMetaData().Add("meta1", "value1");
            o.GetMetaData().Add("meta2", "value2");
            return o;
        }

        private Context CreateContext()
        {
            return new Context(new Uri("http://fakehost:1234/", UriKind.Absolute));
        }

        private IConfiguration CreateConfiguration()
        {
            var mapping = new ResourceMapping<SampleClass, DummyController>(c => c.Id);
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);

            var derivedMapping = new ResourceMapping<DerivedClass, DummyController>(c => c.Id);
            derivedMapping.ResourceType = "derivedClasses";
            derivedMapping.AddPropertyGetter("someValue", c => c.SomeValue);
            derivedMapping.AddPropertyGetter("date", c => c.DateTime);
            derivedMapping.AddPropertyGetter("derivedProperty", c => c.DerivedProperty);

            var mappingWithMeta = new ResourceMapping<SampleClassWithMetadata, DummyController>(c => c.Id);
            mappingWithMeta.ResourceType = "sampleClassesWithMeta";
            mappingWithMeta.AddPropertyGetter("someValue", c => c.SomeValue);

            var config = new NJsonApi.Configuration();
            config.AddMapping(mapping);
            config.AddMapping(derivedMapping);
            config.AddMapping(mappingWithMeta);
            return config;
        }

        private class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public DateTime DateTime { get; set; }
            public string NotMappedValue { get; set; }
        }

        private class DerivedClass : SampleClass
        {
            public string DerivedProperty { get; set; }
        }

        private class SampleClassWithMetadata : IMetaDataContainer
        {
            private MetaData _metaData = new MetaData();

            public int Id { get; set; }
            public string SomeValue { get; set; }

            public MetaData GetMetaData()
            {
                return _metaData;
            }
        }
    }
}