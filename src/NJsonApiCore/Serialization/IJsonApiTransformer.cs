using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using System;

namespace NJsonApi.Serialization
{
    public interface IJsonApiTransformer
    {
        CompoundDocument Transform(Exception e, int httpStatus);

        CompoundDocument Transform(object objectGraph, Context context);

        IDelta TransformBack(UpdateDocument updateDocument, Type type, Context context);
    }
}