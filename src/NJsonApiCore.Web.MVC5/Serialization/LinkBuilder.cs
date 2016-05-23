using NJsonApi.Serialization;
using NJsonApi.Serialization.Representations;
using NJsonApiCore.Web.MVC5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace NJsonApi.Web.MVC5.Serialization
{
    public class LinkBuilder : ILinkBuilder
    {
        private readonly HttpConfiguration configuration;
        private readonly IApiExplorer descriptionProvider;

        public LinkBuilder(HttpConfiguration configuration, IApiExplorer descriptionProvider)
        {
            this.configuration = configuration;
            this.descriptionProvider = descriptionProvider;
        }

        public ILink FindResourceSelfLink(Context context, string resourceId, IResourceMapping resourceMapping)
        {
            var actions = descriptionProvider.From(resourceMapping.Controller);

            var action = actions.Single(a =>
                a.HttpMethod.Method == "GET" &&
                a.ParameterDescriptions.Count(p => p.Name == "id") == 1);

            var formattedUri = action.RelativePath.Replace("{id}", resourceId);

            return new SimpleLink(new Uri(context.BaseUri, formattedUri));
        }

        // TODO - Move into NJsonApiCore base library from here and .mVC6
        public ILink RelationshipRelatedLink(Context context, string resourceId, IResourceMapping resourceMapping, IRelationshipMapping linkMapping)
        {
            var selfLink = FindResourceSelfLink(context, resourceId, resourceMapping).Href;
            var completeLink = $"{selfLink}/{linkMapping.RelationshipName}";
            return new SimpleLink(new Uri(completeLink));
        }

        // TODO - Move into NJsonApiCore base library from here and .mVC6
        public ILink RelationshipSelfLink(Context context, string resourceId, IResourceMapping resourceMapping, IRelationshipMapping linkMapping)
        {
            var selfLink = FindResourceSelfLink(context, resourceId, resourceMapping).Href;
            var completeLink = $"{selfLink}/relationships/{linkMapping.RelationshipName}";
            return new SimpleLink(new Uri(completeLink));
        }
    }
}