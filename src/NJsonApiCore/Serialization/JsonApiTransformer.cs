using Newtonsoft.Json;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Relationships;
using NJsonApi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Serialization
{
    public class JsonApiTransformer : IJsonApiTransformer
    {
        private JsonSerializer serializer;

        private readonly TransformationHelper transformationHelper;
        private readonly IConfiguration configuration;

        public JsonApiTransformer(
            JsonSerializer serializer,
            IConfiguration configuration,
            TransformationHelper transformationHelper)
        {
            this.serializer = serializer;
            this.configuration = configuration;
            this.transformationHelper = transformationHelper;
        }

        public CompoundDocument Transform(Exception e, int httpStatus)
        {
            var result = new CompoundDocument();
            result.Errors = new List<Error>()
            {
                new Error()
                {
                    Title = "There has been an unhandled error when processing your request.",
                    Detail = e.Message,
                    Code = e.ToString(),
                    Status = httpStatus
                }
            };

            return result;
        }

        public CompoundDocument Transform(object objectGraph, Context context)
        {
            Type innerObjectType = Reflection.GetObjectType(objectGraph);

            transformationHelper.VerifyTypeSupport(innerObjectType);
            transformationHelper.AssureAllMappingsRegistered(innerObjectType, configuration);

            var result = new CompoundDocument
            {
                Meta = transformationHelper.GetMetadata(objectGraph)
            };

            var resource = transformationHelper.UnwrapResourceObject(objectGraph);
            var resourceMapping = configuration.GetMapping(innerObjectType);

            var resourceList = transformationHelper.UnifyObjectsToList(resource);
            var representationList = resourceList.Select(
                o => transformationHelper.CreateResourceRepresentation(o, resourceMapping, context));
            result.Data = transformationHelper.ChooseProperResourceRepresentation(resource, representationList);
            result.Links = transformationHelper.GetTopLevelLinks(objectGraph, context.RequestUri);

            if (resourceMapping.Relationships.Any())
            {
                result.Included = transformationHelper.CreateIncludedRepresentations(resourceList, resourceMapping, context);
            }

            return result;
        }

        public IDelta TransformBack(UpdateDocument updateDocument, Type type, Context context)
        {
            var mapping = configuration.GetMapping(type);
            var openGeneric = typeof(Delta<>);
            var closedGenericType = openGeneric.MakeGenericType(type);
            var delta = Activator.CreateInstance(closedGenericType) as IDelta;

            if (delta == null)
            {
                return null;
            }

            delta.ObjectPropertyValues = mapping.GetValuesFromAttributes(updateDocument.Data.Attributes);

            if (updateDocument.Data.Relationships != null)
            {
                foreach (var relMapping in mapping.Relationships)
                {
                    var relatedTypeMapping = configuration.GetMapping(relMapping.RelatedBaseType);
                    if (!updateDocument.Data.Relationships.ContainsKey(relMapping.RelationshipName))
                    {
                        continue;
                    }
                    var relationship = updateDocument.Data.Relationships[relMapping.RelationshipName];

                    if (relMapping.IsCollection && relationship.Data is MultipleResourceIdentifiers)
                    {
                        var multipleIDs = (MultipleResourceIdentifiers)relationship.Data;
                        var openGenericCollection = typeof(CollectionDelta<>);
                        var closedGenericTypeCollection = openGenericCollection.MakeGenericType(relMapping.RelatedBaseType);
                        var collection = Activator.CreateInstance(closedGenericTypeCollection, relatedTypeMapping.IdGetter) as ICollectionDelta;
                        var openGenericList = typeof(List<>);
                        var closedGenericTypeList = openGenericList.MakeGenericType(relMapping.RelatedBaseType);
                        collection.Elements = Activator.CreateInstance(closedGenericTypeList) as IList;
                        foreach (var id in multipleIDs)
                        {
                            var colProp = relMapping.RelatedCollectionProperty;

                            var newInstance = Activator.CreateInstance(relMapping.RelatedBaseType);
                            relatedTypeMapping.IdSetter(newInstance, id.Id);
                            (collection.Elements as IList).Add(newInstance);
                        }
                        delta.CollectionDeltas.Add(relMapping.RelationshipName, collection);
                    }
                    else if (!relMapping.IsCollection && relationship.Data is SingleResourceIdentifier)
                    {
                        var singleId = relationship.Data as SingleResourceIdentifier;
                        object instance;
                        try
                        {
                            instance = Activator.CreateInstance(relMapping.RelatedBaseType);
                        }
                        catch (MissingMethodException ex)
                        {
                            throw new Exception("Could not create the resource with the link given, ensure that you have a parameterless constructor on the linked entity", ex);
                        }
                        relatedTypeMapping.IdSetter(instance, singleId.Id);
                        delta.ObjectPropertyValues.Add(relMapping.RelationshipName, instance);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            if (updateDocument.MetaData?.Count > 0)
            {
                delta.TopLevelMetaData = updateDocument.MetaData;
            }
            if (updateDocument.Data.MetaData?.Count > 0)
            {
                delta.ObjectMetaData = updateDocument.Data.MetaData;
            }

            delta.Scan();

            return delta;
        }
    }
}