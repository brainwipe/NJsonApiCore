using NJsonApi.Infrastructure;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApiCore.Test.TestModel;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestTransformBack
    {
        [Fact]
        public void Transform_properties_with_reserverd_keyword()
        {
            var updateDocument = new UpdateDocument()
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" }
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder()
                .With(config)
                .Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Title"));
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_TwoFields()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" },
                        {"authorId", "1234" },
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder().With(config).Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Title"));
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("AuthorId"));
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_Containing_ComplexObject()
        {
            // Arrange
            var dimensions = new Dimensions() {Height = "80mm", Width = "10mm"};
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "product",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"name", "Widget" },
                        {"dimensions", dimensions }
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder().With(config).Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Product), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Name"));
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Dimensions"));
            Assert.Equal(dimensions, resultDelta.ObjectPropertyValues["Dimensions"]);
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_Containing_ListAttribute()
        {
            //DS: TODO - should widgetParts be provided as a JArray?
            // Arrange
            var widgetParts = new List<WidgetPart>()
            {
                new WidgetPart() { PartNumber = "WIDGET-001" },
                new WidgetPart() { PartNumber = "WIDGET-002" }
            };
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "widget",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"name", "A widget" },
                        {"parts", widgetParts }
                    }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder().With(config).Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Widget), context);

            // Assert
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Name"));
            Assert.True(resultDelta.ObjectPropertyValues.ContainsKey("Parts"));
            Assert.Equal(resultDelta.ObjectPropertyValues["Parts"], widgetParts);
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_ObjectMetaData()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" },
                        {"authorId", "1234" },
                    },
                    MetaData = new MetaData() { { "meta1", "value1"}, { "meta2", "value2"} }
                }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder().With(config).Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.ObjectMetaData.ContainsKey("meta1"));
            Assert.Equal("value1", resultDelta.ObjectMetaData["meta1"]);
            Assert.True(resultDelta.ObjectMetaData.ContainsKey("meta2"));
            Assert.Equal("value2", resultDelta.ObjectMetaData["meta2"]);
        }

        [Fact]
        public void Transform_UpdateDocument_To_Delta_TopLevelMetaData()
        {
            // Arrange
            var updateDocument = new UpdateDocument
            {
                Data = new SingleResource()
                {
                    Id = "123",
                    Type = "post",
                    Attributes = new Dictionary<string, object>()
                    {
                        {"title", "someTitle" },
                        {"authorId", "1234" },
                    }
                },
                MetaData = new MetaData() { { "meta1", "value1" }, { "meta2", "value2" } }
            };

            var config = TestModelConfigurationBuilder.BuilderForEverything.Build();
            var context = new Context(new Uri("http://fakehost:1234", UriKind.Absolute));
            var transformer = new JsonApiTransformerBuilder().With(config).Build();

            // Act
            var resultDelta = transformer.TransformBack(updateDocument, typeof(Post), context);

            // Assert
            Assert.True(resultDelta.TopLevelMetaData.ContainsKey("meta1"));
            Assert.Equal("value1", resultDelta.TopLevelMetaData["meta1"]);
            Assert.True(resultDelta.TopLevelMetaData.ContainsKey("meta2"));
            Assert.Equal("value2", resultDelta.TopLevelMetaData["meta2"]);
        }

        private class PostUpdateOneField
        {
            public string Title { get; set; }
        }

        private class PostUpdateTwoFields
        {
            public int AuthorId { get; set; }
            public string Title { get; set; }
        }
    }
}