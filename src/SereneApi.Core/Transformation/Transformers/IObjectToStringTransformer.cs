namespace SereneApi.Core.Transformation.Transformers
{
    public interface IObjectToStringTransformer
    {
        string TransformValue(object value);
    }
}