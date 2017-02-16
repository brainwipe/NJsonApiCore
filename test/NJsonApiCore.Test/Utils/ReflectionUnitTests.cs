using NJsonApi.Test.TestModel;
using NJsonApi.Utils;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Utils
{
    public class ReflectionUnitTests
    {

        [Fact]
        public void GIVEN_TypeThatIsNotGeneric_WHEN_FromWithinGeneric_THEN_TypeReturned()
        {
            // Arrange
            var nonGenericType = typeof(Post);

            // Act
            var result = Reflection.FromWithinGeneric(nonGenericType);

            // Assert
            Assert.Equal(nonGenericType, result[0]);
        }

        [Fact]
        public void GIVEN_GenericTypeWithMoreThanOneParameter_WHEN_FromWithinGeneric_THEN_TypeReturned()
        {
            // Arrange
            var genericType = typeof(Dictionary<string, Post>);

            // Act
            var result = Reflection.FromWithinGeneric(genericType);

            // Assert
            Assert.Equal(typeof(string), result[0]);
            Assert.Equal(typeof(Post), result[1]);
        }

        [Fact]
        public void GIVEN_NullType_WHEN_FromWithinGeneric_THEN_Exception()
        {
            // Arrange
            Type nullType = null;

            // Act - bang
            Assert.Throws<ArgumentNullException>(() => Reflection.FromWithinGeneric(nullType));
        }

        [Fact]
        public void Given_ArrayType_WHEN_GetObjectType_THEN_ArrayElementTypeReturned()
        {
            // Arrange
            var myEmptyObject = new string[0];

            // Act
            var result = Reflection.GetObjectType(myEmptyObject);

            // Assert
            Assert.Equal(typeof(string), result);
        }

        [Fact]
        public void Given_GenericEnumerable_WHEN_GetObjectType_THEN_GenericTypeReturned()
        {
            // Arrange
            var myStringListObject = new List<string>();

            // Act
            var result = Reflection.GetObjectType(myStringListObject);

            // Assert
            Assert.Equal(typeof(string), result);
        }


        [Fact]
        public void Given_NonEnumerable_WHEN_GetObjectType_THEN_TypeReturned()
        {
            // Arrange
            string myEmptyObject = string.Empty;

            // Act
            var result = Reflection.GetObjectType(myEmptyObject);

            // Assert
            Assert.Equal(typeof(string), result);
        }
    }
}
