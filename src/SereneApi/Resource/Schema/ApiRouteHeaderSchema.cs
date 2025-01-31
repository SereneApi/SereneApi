namespace SereneApi.Resource.Schema
{
    internal readonly struct ApiRouteHeaderSchema
    {
        public string Key { get; }

        public string Value { get; }

        public ApiRouteHeaderSchema(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
