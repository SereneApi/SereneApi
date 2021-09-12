using SereneApi.Core.Content;
using SereneApi.Core.Content.Types;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Serialization
{
    public class SoapSerializer : ISoapSerializer
    {
        private readonly ISoapSerializerSettings _settings;
        private readonly IEnvelopeTranslator _translator;

        public SoapSerializer(IEnvelopeTranslator translator, ISoapSerializerSettings settings)
        {
            _translator = translator;
            _settings = settings;
        }

        public T Deserialize<T>(IResponseContent content)
        {
            Type[] extraTypes = new Type[_settings.ExtraTypes.Count + 1];

            extraTypes[0] = typeof(T);

            int index = 1;

            foreach (Type extraType in _settings.ExtraTypes)
            {
                extraTypes[index] = extraType;

                index++;
            }

            XmlSerializer serializer = new(_settings.ResponseEnvelopmentType, extraTypes);

            ISoapEnvelope value = (ISoapEnvelope)serializer.Deserialize(content.GetContentStreamAsync().Result);

            return _translator.Unpack<T>(value);
        }

        public async Task<T> DeserializeAsync<T>(IResponseContent content)
        {
            Type[] extraTypes = new Type[_settings.ExtraTypes.Count + 1];

            extraTypes[0] = typeof(T);

            int index = 1;

            foreach (Type extraType in _settings.ExtraTypes)
            {
                extraTypes[index] = extraType;

                index++;
            }

            XmlSerializer serializer = new(_settings.ResponseEnvelopmentType, extraTypes);

            ISoapEnvelope value = (ISoapEnvelope)serializer.Deserialize(await content.GetContentStreamAsync());

            return _translator.Unpack<T>(value);
        }

        public IRequestContent Serialize(object value)
        {
            return new XmlContent(SerializeToString(value));
        }

        public string SerializeToString(object value)
        {
            XmlSerializer serializer = new(_settings.RequestEnvelopmentType, _settings.ExtraTypes.ToArray());

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = _settings.OmitXmlDeclaration,
                Indent = _settings.EnableIndentation
            };

            using StringWriter stream = new StringWriter();
            using XmlWriter writer = XmlWriter.Create(stream, settings);

            serializer.Serialize(writer, value, _settings.Namespaces);

            return stream.ToString();
        }
    }
}