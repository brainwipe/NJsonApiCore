using Newtonsoft.Json;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestControllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestSerialization
    {
        [Fact]
        public void Serilized_properly()
        {
            // Arrange
            var context = CreateContext();
            var objectToTransform = CreateObjectToTransform();
            var transformer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            var transformed = transformer.Transform(objectToTransform, context);

            // Act
            var json = JsonConvert.SerializeObject(transformed);

            // Assert
            Assert.DoesNotContain(json, "Data");
            Assert.True(json.Contains("\"complexAttribute\":{\"Label\":\"This is complex attribute class\",\"InnerComplexAttribute\":{\"AnotherLabel\":\"This is inner complex attribute class\"}}"));
            Assert.True(json.Contains("\"listAttribute\":[{\"Label\":\"Complex 1\",\"InnerComplexAttribute\":{\"AnotherLabel\":\"This is inner complex attribute class\"}},{\"Label\":\"Complex 2\",\"InnerComplexAttribute\":{\"AnotherLabel\":\"This is inner complex attribute class\"}}]"));
        }

        private static SampleClass CreateObjectToTransform()
        {
            var innerComplexAttribute = new InnerComplexAttributeClass()
            {
                AnotherLabel = "This is inner complex attribute class"
            };

            var complexAttribute = new ComplexAttributeClass()
            {
                Label = "This is complex attribute class",
                InnerComplexAttribute = innerComplexAttribute
            };

            var listAttribute = new List<ComplexAttributeClass>()
            {
                new ComplexAttributeClass()
                {
                    Label = "Complex 1",
                    InnerComplexAttribute = innerComplexAttribute
                },
                new ComplexAttributeClass()
                {
                    Label = "Complex 2",
                    InnerComplexAttribute = innerComplexAttribute
                }
            };

            var deepest = new DeeplyNestedClass()
            {
                Id = 100,
                Value = "A value"
            };

            return new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped",
                ComplexAttribute = complexAttribute,
                ListAttribute = listAttribute,
                NestedClass = new List<NestedClass>()
                {
                    new NestedClass()
                    {
                        Id = 200,
                        NestedText = "Some nested text",
                        DeeplyNestedClass = deepest
                    },
                    new NestedClass()
                    {
                        Id = 200,
                        NestedText = "Some nested text",
                        DeeplyNestedClass = deepest
                    },
                    new NestedClass()
                    {
                        Id = 201,
                        NestedText = "Some nested text"
                    }
                }
            };
        }

        private Context CreateContext()
        {
            return new Context(new Uri("http://fakehost:1234/", UriKind.Absolute));
        }

        private IConfiguration CreateConfiguration()
        {
            var conf = new NJsonApi.Configuration();
            var sampleClassMapping = new ResourceMapping<SampleClass, DummyController>(c => c.Id);
            sampleClassMapping.ResourceType = "sampleClasses";
            sampleClassMapping.AddPropertyGetter("someValue", c => c.SomeValue);
            sampleClassMapping.AddPropertyGetter("date", c => c.DateTime);
            sampleClassMapping.AddPropertyGetter("complexAttribute", c => c.ComplexAttribute);
            sampleClassMapping.AddPropertyGetter("listAttribute", c => c.ListAttribute);

            var nestedClassMapping = new ResourceMapping<NestedClass, DummyController>(c => c.Id);
            nestedClassMapping.ResourceType = "nestedClasses";
            nestedClassMapping.AddPropertyGetter("nestedText", c => c.NestedText);

            var deeplyNestedMapping = new ResourceMapping<DeeplyNestedClass, DummyController>(c => c.Id);
            deeplyNestedMapping.ResourceType = "deeplyNestedClasses";
            deeplyNestedMapping.AddPropertyGetter("value", c => c.Value);

            var linkMapping = new RelationshipMapping<SampleClass, NestedClass>()
            {
                IsCollection = true,
                RelationshipName = "nestedValues",
                ResourceMapping = nestedClassMapping,
                ResourceGetter = c => c.NestedClass,
            };

            var deepLinkMapping = new RelationshipMapping<NestedClass, DeeplyNestedClass>()
            {
                RelationshipName = "deeplyNestedValues",
                ResourceMapping = deeplyNestedMapping,
                ResourceGetter = c => c.DeeplyNestedClass
            };

            sampleClassMapping.Relationships.Add(linkMapping);
            nestedClassMapping.Relationships.Add(deepLinkMapping);

            conf.AddMapping(sampleClassMapping);
            conf.AddMapping(nestedClassMapping);
            conf.AddMapping(deeplyNestedMapping);

            return conf;
        }

        private class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public DateTime DateTime { get; set; }
            public string NotMappedValue { get; set; }
            public ComplexAttributeClass ComplexAttribute { get; set; }
            public List<ComplexAttributeClass> ListAttribute { get; set; }
            public IEnumerable<NestedClass> NestedClass { get; set; }
        }

        private class NestedClass
        {
            public int Id { get; set; }
            public string NestedText { get; set; }
            public DeeplyNestedClass DeeplyNestedClass { get; set; }
        }

        private class DeeplyNestedClass
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        private class ComplexAttributeClass
        {
            public string Label { get; set; }
            public InnerComplexAttributeClass InnerComplexAttribute { get; set; }
        }

        public class InnerComplexAttributeClass
        {
            public string AnotherLabel { get; set; }
        }
    }
}