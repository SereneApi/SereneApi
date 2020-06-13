namespace SereneApi.Types
{
    public readonly struct MediaType
    {
        public string TypeString { get; }

        public MediaType(string typeString)
        {
            TypeString = typeString;
        }

        public static MediaType ApplicationJson => new MediaType("application/json");

        public static bool operator ==(MediaType typeA, MediaType typeB)
        {
            return typeA.TypeString == typeB.TypeString;
        }

        public static bool operator !=(MediaType typeA, MediaType typeB)
        {
            return typeA.TypeString != typeB.TypeString;
        }
    }
}
