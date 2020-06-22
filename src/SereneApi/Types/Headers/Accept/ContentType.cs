namespace SereneApi.Types.Headers.Accept
{
    public readonly struct ContentType
    {
        public string Value { get; }

        public ContentType(string value)
        {
            Value = value;
        }

        public static ContentType Json => new ContentType("application/json");

        public static ContentType FromUrlEncoded => new ContentType("application/x-www-form-urlencoded");
    }
}
