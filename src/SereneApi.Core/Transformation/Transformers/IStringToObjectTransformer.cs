using System;

namespace SereneApi.Core.Transformation.Transformers
{
    public interface IStringToObjectTransformer
    {
        object TransformValue(string value, Type toType);
    }
}