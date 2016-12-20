using NJsonApi.Web.MVCCore.HelloWorld.Controllers;
using NJsonApi.Web.MVCCore.HelloWorld.Models;
using NJsonApiCore.Web.MVCCore.HelloWorld.Controllers;
using NJsonApiCore.Web.MVCCore.HelloWorld.Models;

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

            configBuilder
                .Resource<Report, ReportsController>()
                .WithAllProperties();

            configBuilder
                .Resource<StatisticsReport, StatisticsReportController>()
                .WithAllProperties();

            var nJsonApiConfig = configBuilder.Build();
            return nJsonApiConfig;
        }
    }
}