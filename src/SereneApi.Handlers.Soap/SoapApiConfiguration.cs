using DeltaWare.Dependencies.Abstractions;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Serialization.Factories;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class SoapApiConfiguration
    {
        public static void AddEnvelopeTranslator<TTranslator>(this IApiConfiguration factory, TTranslator translator = default) where TTranslator : IEnvelopeTranslator
        {
            if (translator == null)
            {
                factory.Dependencies.AddSingleton<IEnvelopeTranslator, TTranslator>();
            }
            else
            {
                factory.Dependencies.AddSingleton<IEnvelopeTranslator>(() => translator);
            }
        }

        public static void ConfigureSoapSettings(this IApiConfiguration configuration, Action<ISoapSerializerSettingsFactory> settings)
        {
            configuration.Dependencies.AddSingleton(() =>
            {
                SoapSerializerSettingsBuilder builder = new SoapSerializerSettingsBuilder();

                settings.Invoke(builder);

                return builder.BuildSerializerSettings();
            });
        }
    }
}