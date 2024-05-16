
// ReSharper disable once CheckNamespace
namespace System
{
    internal static class TypeExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type type, Type genericType)
        {
            if (!genericType.IsGenericType)
            {
                throw new ArgumentException();
            }

            if (type == genericType)
            {
                return false;
            }

            while (type.BaseType != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }

                if (type == genericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
