using SereneApi.Handlers.Soap.Models;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Serialization
{
    public class SoapSerializerSettings : ISoapSerializerSettings
    {
        public bool EnableIndentation { get; set; } = true;

        public List<Type> ExtraTypes { get; set; } = new();

        public XmlSerializerNamespaces Namespaces { get; set; } = new();

        public bool OmitXmlDeclaration { get; set; } = true;

        public Type RequestEnvelopmentType { get; set; } = typeof(SoapEnvelope);

        public Type ResponseEnvelopmentType { get; set; } = typeof(SoapEnvelope);

        public SoapSerializerSettings()
        {
            Namespaces.Add("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
        }
    }
}