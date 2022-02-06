using SereneApi.Handlers.Soap.Envelopment.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Serialization.Factories
{
    public class SoapSerializerSettingsBuilder : ISoapSerializerSettingsFactory
    {
        private readonly SoapSerializerSettings _settings = new();

        public XmlSerializerNamespaces Namespaces => _settings.Namespaces;

        public void AddEnvelope<TEnvelope>() where TEnvelope : class
        {
            _settings.ExtraTypes.Add(typeof(TEnvelope));
        }

        public void AddEnvelope(params Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (types.Length == 0)
            {
                throw new ArgumentException("At least one extra type must be provided", nameof(types));
            }

            _settings.ExtraTypes.AddRange(types);
        }

        public void AddEnvelopesFromAssembly<TEnvelope>() where TEnvelope : class
        {
            AddEnvelopesFromAssembly(typeof(TEnvelope));
        }

        public void AddEnvelopesFromAssembly(params Type[] assemblyMakerTypes)
        {
            if (assemblyMakerTypes == null)
            {
                throw new ArgumentNullException(nameof(assemblyMakerTypes));
            }

            if (assemblyMakerTypes.Length == 0)
            {
                throw new ArgumentException("Must provided at least one or more assembly marker types",
                    nameof(assemblyMakerTypes));
            }

            Type[] types = assemblyMakerTypes
                .SelectMany(a => a.Assembly.GetLoadedTypes())
                .Where(t => t.GetCustomAttribute<EnvelopeAttribute>() != null && !t.IsAbstract)
                .ToArray();

            AddEnvelope(types);
        }

        public ISoapSerializerSettings BuildSerializerSettings()
        {
            return _settings;
        }

        public void EnableIndentation()
        {
            _settings.EnableIndentation = true;
        }

        public void OmitXmlDeclaration()
        {
            _settings.OmitXmlDeclaration = true;
        }

        public void OverrideRequestEnvelope<TEnvelope>() where TEnvelope : class
        {
            _settings.RequestEnvelopmentType = typeof(TEnvelope);
        }

        public void OverrideResponseEnvelope<TEnvelope>() where TEnvelope : class
        {
            _settings.ResponseEnvelopmentType = typeof(TEnvelope);
        }
    }
}