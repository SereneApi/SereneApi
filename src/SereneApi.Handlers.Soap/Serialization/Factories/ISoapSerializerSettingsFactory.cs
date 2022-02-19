using SereneApi.Handlers.Soap.Envelopment.Attributes;
using System;
using System.Xml.Serialization;

namespace SereneApi.Handlers.Soap.Serialization.Factories
{
    public interface ISoapSerializerSettingsFactory
    {
        XmlSerializerNamespaces Namespaces { get; }

        void AddEnvelope<TEnvelope>() where TEnvelope : class;

        void AddEnvelope(params Type[] types);

        /// <summary>
        /// Adds types marked with the <see cref="EnvelopeAttribute"/> from the provided types assembly.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null array is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when an empty array is provided.</exception>
        void AddEnvelopesFromAssembly<TEnvelope>() where TEnvelope : class;

        /// <summary>
        /// Adds types marked with the <see cref="EnvelopeAttribute"/> from each of the provided
        /// types assembly.
        /// </summary>
        /// <param name="assemblyMakerTypes">Each type listed will have its assembly scanned.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null array is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when an empty array is provided.</exception>
        void AddEnvelopesFromAssembly(params Type[] assemblyMakerTypes);

        void EnableIndentation();

        void OmitXmlDeclaration();

        void OverrideRequestEnvelope<TEnvelope>() where TEnvelope : class;

        void OverrideResponseEnvelope<TEnvelope>() where TEnvelope : class;
    }
}