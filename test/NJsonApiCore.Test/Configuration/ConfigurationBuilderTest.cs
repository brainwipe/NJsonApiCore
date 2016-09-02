using NJsonApi.Conventions.Impl;
using NJsonApi.Test.TestControllers;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NJsonApi.Test.Configuration
{
    public class ConfigurationBuilderTest
    {
        [Fact]
        public void Resource_creates_mapping()
        {
            //Arrange
            var builder = new ConfigurationBuilder();

            //Act
            builder.Resource<Post, PostsController>();

            var result = builder.Build();

            //Assert
            Assert.True(result.IsResourceRegistered(typeof(Post)));
            Assert.NotNull(result.GetMapping(typeof(Post)));
        }

        [Fact]
        public void WithSimpleProperty_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            var post = new Post() { Title = "test" };

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithSimpleProperty(p => p.Title);

            var configuration = builder.Build();
            var mapping = configuration.GetMapping(typeof(Post));

            //Assert
            Assert.Equal(mapping.PropertyGetters.Count, 1);
            Assert.Equal(mapping.PropertySetters.Count, 1);

            var getter = mapping.PropertyGetters.Single().Value;
            var setter = mapping.PropertySetters.Single().Value;

            Assert.Equal(((string)getter(post)), "test");

            setter(post, "works");
            Assert.Equal(post.Title, "works");
        }

        [Fact]
        public void WithIdSelector_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            var post = new Post { Id = 4 };

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithIdSelector(p => p.Id);

            var configuration = builder.Build();
            var mapping = configuration.GetMapping(typeof(Post));

            //Assert
            Assert.NotNull(mapping.IdGetter);
            Assert.Equal(mapping.IdGetter(post), 4);
        }

        [Fact]
        public void WithLinkedResource_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());

            var post = new Post();
            var author = new Author
            {
                Posts = new List<Post> { post }
            };

            post.Author = author;
            post.AuthorId = 4;

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithLinkedResource(p => p.Author);

            builder
                .Resource<Author, AuthorsController>()
                .WithLinkedResource(a => a.Posts);

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));
            var authorMapping = configuration.GetMapping(typeof(Author));

            //Assert
            Assert.Equal(postMapping.Relationships.Count, 1);

            var linkToAuthor = postMapping.Relationships.Single();

            Assert.False(linkToAuthor.IsCollection);
            Assert.Equal(linkToAuthor.RelationshipName, "author");
            Assert.Equal(linkToAuthor.ParentType, typeof(Post));
            Assert.Equal(linkToAuthor.RelatedBaseType, typeof(Author));
            Assert.Same(linkToAuthor.RelatedResource(post), author);
            Assert.Equal(linkToAuthor.RelatedResourceId(post), 4);
            Assert.Same(linkToAuthor.ResourceMapping, authorMapping);
            Assert.Equal(authorMapping.Relationships.Count, 1);

            var linkToPosts = authorMapping.Relationships.Single();

            Assert.True(linkToPosts.IsCollection);
            Assert.Equal(linkToPosts.RelationshipName, "posts");
            Assert.Equal(linkToPosts.ParentType, typeof(Author));
            Assert.Equal(linkToPosts.RelatedBaseType, typeof(Post));
            Assert.Same(linkToPosts.RelatedResource(author), author.Posts);
            Assert.Null(linkToPosts.RelatedResourceId);
            Assert.Same(linkToPosts.ResourceMapping, postMapping);
        }

        [Fact]
        public void GIVEN_ConfigurationWithOneResource_WHEN_Build_THEN_ConfigurationIsCorrect()
        {
            //Arrange
            var builder = new ConfigurationBuilder();

            var testTitle = "test";
            var post = new Post
            {
                Id = 4,
                Title = testTitle
            };

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithAllSimpleProperties();

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            Assert.NotNull(postMapping.IdGetter);
            Assert.Equal(postMapping.IdGetter(post), 4);
            Assert.Equal(postMapping.PropertyGetters.Count, 2);
            Assert.Equal(postMapping.PropertySetters.Count, 2);
            Assert.Equal(postMapping.PropertyGetters["Title"](post), testTitle);
            Assert.True(postMapping.PropertyGetters.ContainsKey("AuthorId"));
        }

        [Fact]
        public void WithAllLinkedResources_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new DefaultPropertyScanningConvention())
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithAllLinkedResources();

            builder.Resource<Author, AuthorsController>();
            builder.Resource<Comment, CommentsController>();

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            Assert.Equal(postMapping.Relationships.Count, 2);
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "authors"));
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "comments"));
        }

        [Fact]
        public void WithAllProperties_maps_properly()
        {
            //Arrange
            var builder = new ConfigurationBuilder();
            builder
                .WithConvention(new DefaultPropertyScanningConvention())
                .WithConvention(new CamelCaseLinkNameConvention())
                .WithConvention(new PluralizedCamelCaseTypeConvention())
                .WithConvention(new SimpleLinkedIdConvention());

            //Act
            builder
                .Resource<Post, PostsController>()
                .WithAllProperties();

            builder.Resource<Author, AuthorsController>();
            builder.Resource<Comment, CommentsController>();

            var configuration = builder.Build();
            var postMapping = configuration.GetMapping(typeof(Post));

            //Assert
            Assert.Equal(postMapping.Relationships.Count, 2);
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "authors"));
            Assert.NotNull(postMapping.Relationships.SingleOrDefault(l => l.RelatedBaseResourceType == "comments"));
            Assert.Equal(postMapping.PropertyGetters.Count, 2);
            Assert.Equal(postMapping.PropertySetters.Count, 2);
            Assert.NotNull(postMapping.IdGetter);
        }

        [Fact]
        public void WithComplexObjectTest()
        {
            //Arrange
            const int authorId = 5;
            const string authorName = "Valentin";
            const int postId = 6;
            const string postTitle = "The measure of a man";
            const string commentBody = "Comment body";
            const int commentId = 7;
            var author = new Author() { Id = authorId, Name = authorName };
            var post = new Post() { Id = postId, Title = postTitle, Author = author };
            var comment = new Comment() { Id = commentId, Body = commentBody, Post = post };
            post.Replies = new List<Comment>() { comment };
            author.Posts = new List<Post>() { post };

            var configurationBuilder = new ConfigurationBuilder();

            //Act
            var resourceConfigurationForPost = configurationBuilder
                .Resource<Post, PostsController>()
                .WithSimpleProperty(p => p.Title)
                .WithIdSelector(p => p.Id)
                .WithLinkedResource(p => p.Replies);
            var resourceConfigurationForAuthor = configurationBuilder
                .Resource<Author, AuthorsController>()
                .WithSimpleProperty(a => a.Name)
                .WithIdSelector(a => a.Id)
                .WithLinkedResource(a => a.Posts);
            var resourceConfigurationForComment = configurationBuilder
                .Resource<Comment, CommentsController>()
                .WithIdSelector(c => c.Id)
                .WithSimpleProperty(c => c.Body);
            var result = configurationBuilder.Build();

            //Assert
            Assert.Equal(resourceConfigurationForPost.BuiltResourceMapping.Relationships.Count, 1);
            Assert.Equal(resourceConfigurationForAuthor.BuiltResourceMapping.Relationships.Count, 1);
            configurationBuilder.ResourceConfigurationsByType.All(
                r => r.Value.BuiltResourceMapping.Relationships.All(l => l.ResourceMapping != null));
            var authorLinks =
                 configurationBuilder.ResourceConfigurationsByType[
                     resourceConfigurationForAuthor.BuiltResourceMapping.ResourceRepresentationType].BuiltResourceMapping.Relationships;
            Assert.Equal(authorLinks.Count, 1);
            Assert.Equal(authorLinks[0].RelationshipName, "posts");
            Assert.Equal(authorLinks[0].ResourceMapping.PropertyGetters.Count, 1);
        }

        [Fact]
        public void GIVEN_ModelWithReservedWordProperties_WHEN_BuildConfiguration_THEN_Exception()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act - Exception!
            Assert.Throws<InvalidOperationException>(() => configurationBuilder.Resource<BadModelWithReservedWords, DummyController>());
        }

        [Fact]
        public void GIVEN_ValidModel_AND_ChildPropertyHasRervedWordProperties_WHEN_BuildConfiguration_THEN_Exception()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act - Exception!
            Assert.Throws<InvalidOperationException>(() => configurationBuilder.Resource<ValidModelWithBadChild, DummyController>());
        }

        [Fact]
        public void GIVEN_ModelWithSelfRefernce_WHEN_ValidateReservedWords_THEN_NotInfiniteLoop()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act - Will not throw an infinite loop error
            configurationBuilder.Resource<ModelThatCausesInfiniteLoop, DummyController>();
        }
    }
}