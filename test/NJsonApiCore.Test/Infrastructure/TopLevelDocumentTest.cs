using System.Collections.Generic;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Representations;
using Xunit;

namespace NJsonApi.Test.Infrastructure
{
    public class TopLevelDocumentTest
    {
        [Fact]
        public void TopLevelDocument_using_ctor_string_ok()
        {
            // Arrange
            const string testString = "Test String";
            
            // Act
            var sut = new TopLevelDocument<string>(testString);

            // Assert
            Assert.Equal(sut.Value, testString);
            Assert.Empty(sut.GetMetaData());
        }

        [Fact]
        public void TopLevelDocument_add_result_collection_ok()
        {
            // Arrange
            var testsStrings = new List<string>(){ "test1", "test2" };

            // Act
            var sut = new TopLevelDocument<List<string>>(testsStrings);

            // Assert
            Assert.Equal(sut.Value, testsStrings);
            Assert.Empty(sut.GetMetaData());
        }

        [Fact]
        public void TopLevelDocument_add_metadata_ok()
        {
            // Arrange
            const string testString = "Test String";

            // Act
            var sut = new TopLevelDocument<string>(testString);
            sut.GetMetaData().Add("meta1", "value1");

            // Assert
            Assert.Equal("value1", sut.GetMetaData()["meta1"]);
        }

        [Fact]
        public void TopLevelDocument_add_links_ok()
        {
            // Arrange
            const string testString = "Test String";
            var link = new SimpleLink();

            // Act
            var sut = new TopLevelDocument<string>(testString);
            sut.Links.Add("link1", link);

            // Assert
            Assert.Same(link, sut.Links["link1"]);
        }
    }
}