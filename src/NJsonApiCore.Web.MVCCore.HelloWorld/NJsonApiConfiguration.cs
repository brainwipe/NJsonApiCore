using NJsonApi.Web.MVCCore.HelloWorld.Controllers;
using NJsonApi.Web.MVCCore.HelloWorld.Models;

namespace NJsonApi.Web.MVCCore.HelloWorld
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