using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NJsonApi;
using NJsonApi.Test.TestModel;
using NJsonApi.Test.TestControllers;
using NJsonApiCore.Test.TestModel;

namespace NJsonApi.Test.Builders
{
    internal static class TestModelConfigurationBuilder
    {
        public static ConfigurationBuilder BuilderForEverything
        {
            get
            {
                var builder = new ConfigurationBuilder();
                builder
                    .Resource<Post, PostsController>()
                    .WithAllProperties();

                builder
                    .Resource<Author, AuthorsController>()
                    .WithAllProperties();

                builder
                    .Resource<Comment, CommentsController>()
                    .WithAllProperties();

                builder
                    .Resource<Product, ProductsController>()
                    .WithAllSimpleProperties()
                    .WithSimpleProperty(x => x.Dimensions);

                builder
                    .Resource<Widget, WidgetsController>()
                    .WithAllSimpleProperties()
                    .WithSimpleProperty(x => x.Parts);

                return builder;
            }
        }
    }
}
