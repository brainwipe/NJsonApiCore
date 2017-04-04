using NJsonApi.Infrastructure;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Infrastructure
{
    public class DeltaTest
    {
        [Fact]
        public void GIVEN_IncompleteProperties_WHEN_DeltaApply_THEN_OnlyThoseSpecifiedApplied()
        {
            //Arange
            var author = new Author();
            var classUnderTest = new Delta<Author>();

            classUnderTest.FilterOut(t => t.Name);
            classUnderTest.ObjectPropertyValues =
                new Dictionary<string, object>()
                {
                    {"Id", 1},
                    {"DateTimeCreated", new DateTime(2016,1,1)}
                };
            //Act
            classUnderTest.ApplySimpleProperties(author);

            //Assert
            Assert.Equal(author.Id, 1);
            Assert.Equal(author.DateTimeCreated, new DateTime(2016, 1, 1));
            Assert.Null(author.Name);
        }

        [Fact]
        public void GIVEN_NoProperties_WHEN_DeltaApply_THEN_OutputsAreDefault()
        {
            //Arrange
            var simpleObject = new Author();
            var objectUnderTest = new Delta<Author>();

            //Act
            objectUnderTest.ApplySimpleProperties(simpleObject);

            //Assert
            Assert.Equal(simpleObject.Id, 0);
            Assert.Null(simpleObject.Name);
            Assert.Equal(simpleObject.DateTimeCreated, new DateTime());
        }

        [Fact]
        public void GIVEN_SimplePropertiesOfDifferentTypes_WHEN_ApplySimpleProperties_THEN_PropertiesAreApplied()
        {
            //Arrange
            var simpleObject = new SimpleTypes();
            var objectUnderTest = new Delta<SimpleTypes>();

            //Act
            objectUnderTest.ObjectPropertyValues =
                new Dictionary<string, object>()
                {
                    {"TestInt", 154},
                    {"NullableInt", "54"},
                    {"TestDouble", 100},
                    {"NullableDouble", 115.5f}
                };
            objectUnderTest.ApplySimpleProperties(simpleObject);

            //Assert
            Assert.Equal(simpleObject.TestInt, 154);
            Assert.Equal(simpleObject.NullableInt, 54);
            Assert.Equal(simpleObject.TestDouble, 100d);
            Assert.Equal(simpleObject.NullableDouble, 115.5d);
        }
    }
}