using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Serialization
{
    public interface ISoapSerializerSettings
    {
        bool EnableIndentation { get; }
        public List<Type> ExtraTypes { get; }
        public XmlSerializerNamespaces Namespaces { get; }
        bool OmitXmlDeclaration { get; }
        public Type RequestEnvelopmentType { get; }

        public Type ResponseEnvelopmentType { get; }
    }
}