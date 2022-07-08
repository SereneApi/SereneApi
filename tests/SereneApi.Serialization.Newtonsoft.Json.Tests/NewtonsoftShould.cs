using DeltaWare.Dependencies.Abstractions;
using Newtonsoft.Json;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using SereneApi.Core.Http;
using SereneApi.Core.Serialization;
using SereneApi.Serializers.Newtonsoft.Json;
using SereneApi.Serializers.Newtonsoft.Json.Serializers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Serialization.Newtonsoft.Json.Tests
{
    public class NewtonsoftShould
    {
        [Fact]
        public void AddSuccessfully()
        {
            ApiConfigurationManager configuration = new ApiConfigurationManager();

            configuration.AddApiConfiguration<TestHandler>(c =>
            {
                c.SetSource("http://localhost");
            });

            Should.NotThrow(() => configuration.AmendConfigurationProvider<TestHandlerConfigurationProvider>(c =>
            {
                c.UseNewtonsoftSerializer();
            }));

            IApiSettings settings = configuration.BuildApiSettings<TestHandler>();

            ISerializer serializer = settings.Dependencies.GetRequiredDependency<ISerializer>();

            serializer.ShouldBeOfType<NewtonsoftSerializer>();
        }

        [Fact]
        public void AddWithSettingsBuilderSuccessfully()
        {
            string dateFormat = "TEST";

            ApiConfigurationManager configuration = new ApiConfigurationManager();

            Should.NotThrow(() => configuration.AmendConfigurationProvider<TestHandlerConfigurationProvider>(c =>
            {
                c.UseNewtonsoftSerializer(s => s.DateFormatString = dateFormat);
            }));

            configuration.AddApiConfiguration<TestHandler>(c =>
            {
                c.SetSource("http://localhost");
            });

            IApiSettings settings = configuration.BuildApiSettings<TestHandler>();

            ISerializer serializer = settings.Dependencies.GetRequiredDependency<ISerializer>();

            NewtonsoftSerializer newtonsoftSerializer = serializer.ShouldBeOfType<NewtonsoftSerializer>();

            newtonsoftSerializer.DeserializerSettings.DateFormatString.ShouldBe(dateFormat);
            newtonsoftSerializer.SerializerSettings.DateFormatString.ShouldBe(dateFormat);
        }

        [Fact]
        public void AddWithSettingsSuccessfully()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = "TEST"
            };

            ApiConfigurationManager configuration = new ApiConfigurationManager();

            Should.NotThrow(() => configuration.AmendConfigurationProvider<TestHandlerConfigurationProvider>(c =>
            {
                c.UseNewtonsoftSerializer(settings);
            }));

            configuration.AddApiConfiguration<TestHandler>(c =>
            {
                c.SetSource("http://localhost");
            });

            IApiSettings apiSettings = configuration.BuildApiSettings<TestHandler>();

            ISerializer serializer = apiSettings.Dependencies.GetRequiredDependency<ISerializer>();

            NewtonsoftSerializer newtonsoftSerializer = serializer.ShouldBeOfType<NewtonsoftSerializer>();

            newtonsoftSerializer.DeserializerSettings.ShouldBe(settings);
            newtonsoftSerializer.SerializerSettings.ShouldBe(settings);
        }

        [Fact]
        public void ThrowArgumentNullException_Builder()
        {
            ApiConfigurationManager configuration = new ApiConfigurationManager();

            Action<JsonSerializerSettings> builder = null;

            configuration.AmendConfigurationProvider<TestHandlerConfigurationProvider>(c =>
            {
                c.UseNewtonsoftSerializer(builder);
            });

            configuration.AddApiConfiguration(typeof(TestHandler), c =>
            {
                c.SetSource("http://localhost");
            });

            Should.Throw<ArgumentNullException>(() => configuration.BuildApiSettings(typeof(TestHandler)));
        }

        [Fact]
        public void ThrowArgumentNullException_Settings()
        {
            ApiConfigurationManager configuration = new ApiConfigurationManager();

            JsonSerializerSettings settings = null;

            configuration.AmendConfigurationProvider<TestHandlerConfigurationProvider>(c =>
            {
                c.UseNewtonsoftSerializer(settings);
            });

            configuration.AddApiConfiguration(typeof(TestHandler), c =>
            {
                c.SetSource("http://localhost");
            });

            Should.Throw<ArgumentNullException>(() => configuration.BuildApiSettings(typeof(TestHandler)));
        }

        [UseHandlerConfigurationProvider(typeof(TestHandlerConfigurationProvider))]
        public class TestHandler : IApiHandler
        {
            public IConnectionSettings Connection => throw new NotImplementedException();

            public void Dispose()
            {
            }
        }

        public class TestHandlerConfigurationProvider : HandlerConfigurationProvider
        {
            protected override void Configure(IDependencyCollection dependencies)
            {
            }
        }
    }
}