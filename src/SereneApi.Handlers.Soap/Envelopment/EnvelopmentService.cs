using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Content.Types;
using SereneApi.Handlers.Soap.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SereneApi.Handlers.Soap.Envelopment
{
    public class EnvelopmentService : IEnvelopmentService
    {
        private readonly ISoapSerializer _serializer;

        private readonly ISoapSerializerSettings _settings;

        public EnvelopmentService(ISoapSerializer serializer, ISoapSerializerSettings settings)
        {
            _serializer = serializer;
            _settings = settings;
        }

        public IRequestContent Envelop(Dictionary<string, string> parameters, string serviceName, string prefix = null, string namespaceUri = null)
        {
            Type requestType = _settings.RequestEnvelopmentType;

            object defaultRequest = Activator.CreateInstance(requestType);

            string xmlString = _serializer.SerializeToString(defaultRequest);

            XmlDocument document = CreateXmlDocument(xmlString, prefix, serviceName, namespaceUri);

            ProcessParameters(parameters, $"{prefix}:{serviceName}", document);

            return new XmlContent(GenerateXmlString(document));
        }

        private XmlDocument CreateXmlDocument(string xmlString, string prefix, string serviceName, string namespaceUri)
        {
            XmlDocument document = new XmlDocument();

            document.LoadXml(xmlString);

            XmlNodeList nodes = document.GetElementsByTagName("soapenv:Body");

            XmlElement element = document.CreateElement(prefix, serviceName, namespaceUri);

            nodes.Item(0).AppendChild(element);

            return document;
        }

        private string GenerateXmlString(XmlDocument document)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = _settings.EnableIndentation,
                OmitXmlDeclaration = _settings.EnableIndentation
            };

            using StringWriter stringWriter = new StringWriter();
            using XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, settings);

            document.WriteTo(xmlTextWriter);

            xmlTextWriter.Flush();

            return stringWriter.GetStringBuilder().ToString();
        }

        private void ProcessParameters(Dictionary<string, string> parameters, string name, XmlDocument document)
        {
            XmlNode node = document.GetElementsByTagName(name).Item(0);

            //if (node is XmlElement element)
            //{
            //    element.InnerText = parameters.Values.FirstOrDefault();
            //}

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                string key = parameter.Key;

                XmlElement elementParam = document.CreateElement(key);

                elementParam.InnerText = parameter.Value;

                node.AppendChild(elementParam);
            }
        }
    }
}