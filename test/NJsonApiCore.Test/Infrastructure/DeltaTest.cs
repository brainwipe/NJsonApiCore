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
            //Arrange
            var author = new Author();
            var classUnderTest = new Delta<Author>();
            
            classUnderTest.ObjectPropertyValues =
                new Dictionary<string, object>()
                {
                    {"Id", 1},
                    {"DateTimeCreated", new DateTime(2016,1,1)}
                };
            classUnderTest.Scan();
            classUnderTest.FilterOut(t => t.Name);

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
            objectUnderTest.Scan();

            //Act
            objectUnderTest.ApplySimpleProperties(simpleObject);

            //Assert
            Assert.Equal(simpleObject.Id, 0);
            Assert.Null(simpleObject.Name);
            Assert.Equal(simpleObject.DateTimeCreated, new DateTime());
        }

        [Fact]
        public void GIVEN_ScanNotCalled_WHEN_DeltaFilterOut_THEN_ExceptionThrown()
        {
            //Arrange
            var classUnderTest = new Delta<Author>();

            classUnderTest.ObjectPropertyValues =
                new Dictionary<string, object>()
                {
                    {"Id", 1},
                    {"DateTimeCreated", new DateTime(2016,1,1)}
                };

            //Act/Assert
            var ex = Assert.Throws<Exception>(()=> classUnderTest.FilterOut(t => t.Name));
            Assert.Equal("Scan must be called before this method", ex.Message);
        }

        [Fact]
        public void GIVEN_ScanNotCalled_WHEN_DeltaApplySimpleProperties_THEN_ExceptionThrown()
        {
            //Arrange
            var author = new Author();
            var classUnderTest = new Delta<Author>();

            //Act/Assert
            var ex = Assert.Throws<Exception>(() => classUnderTest.ApplySimpleProperties(author));
            Assert.Equal("Scan must be called before this method", ex.Message);
        }

        [Fact]
        public void GIVEN_ScanNotCalled_WHEN_DeltaApplyCollections_THEN_ExceptionThrown()
        {
            //Arrange
            var author = new Author();
            var classUnderTest = new Delta<Author>();

            //Act/Assert
            var ex = Assert.Throws<Exception>(() => classUnderTest.ApplyCollections(author));
            Assert.Equal("Scan must be called before this method", ex.Message);
        }
    }
}