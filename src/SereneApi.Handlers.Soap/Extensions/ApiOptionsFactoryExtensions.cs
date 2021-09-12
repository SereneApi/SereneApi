using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Options.Factories;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Serialization.Factories;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core
{
    public static class ApiOptionsFactoryExtensions
    {
        public static void AddEnvelopeTranslator<TTranslator>(this IApiOptionsFactory factory, TTranslator translator = default) where TTranslator : IEnvelopeTranslator
        {
            ApiOptionsFactory optionsFactory = (ApiOptionsFactory)factory;

            if (translator == null)
            {
                optionsFactory.Dependencies.AddScoped<IEnvelopeTranslator, TTranslator>();
            }
            else
            {
                optionsFactory.Dependencies.AddScoped<IEnvelopeTranslator>(() => translator);
            }
        }

        public static void ConfigureSoapSettings(this IApiOptionsFactory factory, Action<ISoapSerializerSettingsFactory> settings)
        {
            ApiOptionsFactory optionsFactory = (ApiOptionsFactory)factory;

            SoapSerializerSettingsBuilder builder = new SoapSerializerSettingsBuilder();

            settings.Invoke(builder);

            optionsFactory.Dependencies.AddSingleton(() => builder.BuildSerializerSettings());
        }
    }
}