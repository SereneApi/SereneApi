using System;

namespace SereneApi.Types
{
    public readonly struct MediaType
    {
        public string TypeString { get; }

        public static MediaType Json => new MediaType("application/json");

        public static MediaType FromUrlEncoded => new MediaType("application/x-www-form-urlencoded");

        public MediaType(string typeString)
        {
            TypeString = typeString;
        }

        public static bool operator ==(MediaType typeA, MediaType typeB)
        {
            return typeA.TypeString == typeB.TypeString;
        }

        public static bool operator !=(MediaType typeA, MediaType typeB)
        {
            return typeA.TypeString != typeB.TypeString;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is MediaType content))
            {
                return false;
            }

            return TypeString == content.TypeString;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TypeString);
        }
    }
}
