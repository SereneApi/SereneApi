using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SereneApi.Core.Transformation
{
    public interface ITransformationService
    {
        Dictionary<string, string> BuildDictionary<T>(T value) where T : class;

        Dictionary<string, string> BuildDictionary<T>(T value, Expression<Func<T, object>> propertySelector) where T : class;

        T BuildObject<T>(Dictionary<string, string> property);
    }
}