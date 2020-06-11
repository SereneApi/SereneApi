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
    }
}
