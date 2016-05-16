using NJsonApi.Web.MVC6.HelloWorld.Controllers;
using NJsonApi.Web.MVC6.HelloWorld.Models;

namespace NJsonApi.Web.MVC6.HelloWorld
{
    public static class NJsonApiConfiguration
    {
        public static IConfiguration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder
                .Resource<Article, ArticlesController>()
                .WithAllProperties();

            configBuilder
                .Resource<Person, PeopleController>()
                .WithAllProperties();

            configBuilder
                .Resource<Comment, CommentsController>()
                .WithAllProperties();

            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}