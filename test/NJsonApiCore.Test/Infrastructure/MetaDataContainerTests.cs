using System.Collections.Generic;
using NJsonApi.Infrastructure;
using Xunit;

namespace NJsonApi.Test.Infrastructure
{
    public class MetaDataContainerTest
    {
        [Fact]
        public void MetaDataContainer_using_ctor_ok()
        {
            // Arrange

            // Act
            var sut = new MetaDataContainer();

            // Assert
            Assert.Empty(sut.GetMetaData());
        }

        [Fact]
        public void MetaData_add_items_ok()
        {
            // Arrange

            // Act
            var sut = new MetaDataContainer();
            sut.GetMetaData().Add("key", "value");

            // Assert
            Assert.Equal("value", sut.GetMetaData()["key"]);
        }
    }
}