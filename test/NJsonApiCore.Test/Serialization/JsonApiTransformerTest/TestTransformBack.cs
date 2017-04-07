using NJsonApi.Serialization;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
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
            Assert.Equal(resultDelta.ObjectPropertyValues["Dimensions"], dimensions);
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