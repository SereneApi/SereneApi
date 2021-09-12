using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Serialization.Factories;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core
{
    public static class HandlerConfigurationFactoryExtensions
    {
        public static void AddEnvelopeTranslator<TTranslator>(this IHandlerConfigurationFactory factory, TTranslator translator = default) where TTranslator : IEnvelopeTranslator
        {
            ConfigurationFactory optionsFactory = (ConfigurationFactory)factory;

            if (translator == null)
            {
                optionsFactory.Dependencies.AddScoped<IEnvelopeTranslator, TTranslator>();
            }
            else
            {
                optionsFactory.Dependencies.AddScoped<IEnvelopeTranslator>(() => translator);
            }
        }

        public static void ConfigureSoapSettings(this IHandlerConfigurationFactory factory, Action<ISoapSerializerSettingsFactory> settings)
        {
            ConfigurationFactory configurationFactory = (ConfigurationFactory)factory;

            SoapSerializerSettingsBuilder builder = new();

            settings.Invoke(builder);

            configurationFactory.Dependencies.AddSingleton(() => builder.BuildSerializerSettings());
        }
    }
}