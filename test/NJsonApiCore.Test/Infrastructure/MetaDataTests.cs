using System.Collections.Generic;
using NJsonApi.Infrastructure;
using Xunit;

namespace NJsonApi.Test.Infrastructure
{
    public class MetaDataTest
    {
        [Fact]
        public void MetaData_using_ctor_ok()
        {
            // Arrange

            // Act
            var sut = new MetaData();

            // Assert
            Assert.Empty(sut);
        }

        [Fact]
        public void MetaData_add_items_ok()
        {
            // Arrange
            
            // Act
            var sut = new MetaData();
            sut.Add("key", "value");

            // Assert
            Assert.Equal("value", sut["key"]);
        }
    }
}