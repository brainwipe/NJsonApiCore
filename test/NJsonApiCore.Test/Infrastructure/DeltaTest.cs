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
    }
}