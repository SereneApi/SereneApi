using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Models
{
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapEnvelope : ISoapEnvelope
    {
        public object Body { get; set; } = new();
        public object Header { get; set; } = new();
    }
}