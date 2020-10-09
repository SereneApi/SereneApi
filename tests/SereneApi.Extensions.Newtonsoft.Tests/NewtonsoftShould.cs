using DeltaWare.Dependencies.Abstractions;
using Newtonsoft.Json;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Serialization;
using SereneApi.Extensions.Newtonsoft.Serializers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Extensions.Newtonsoft.Tests
{
    public class NewtonsoftShould
    {
        [Fact]
        public void AddSuccessfully()
        {
            IApiConfiguration configuration = new ApiConfiguration();

            IApiConfigurationExtensions configurationExtensions = configuration.GetExtensions();

            Should.NotThrow(() => configurationExtensions.AddNewtonsoft());

            IApiOptionsBuilder options = configuration.GetOptionsBuilder();

            using IDependencyProvider provider = options.Dependencies.BuildProvider();

            ISerializer serializer = provider.GetDependency<ISerializer>();

            serializer.ShouldBeOfType<NewtonsoftSerializer>();
        }

        [Fact]
        public void AddWithSettingsSuccessfully()
        {
            IApiConfiguration configuration = new ApiConfiguration();

            IApiConfigurationExtensions configurationExtensions = configuration.GetExtensions();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = "TEST"
            };

            Should.NotThrow(() => configurationExtensions.AddNewtonsoft(settings));

            IApiOptionsBuilder options = configuration.GetOptionsBuilder();

            using IDependencyProvider provider = options.Dependencies.BuildProvider();

            ISerializer serializer = provider.GetDependency<ISerializer>();

            NewtonsoftSerializer newtonsoftSerializer = serializer.ShouldBeOfType<NewtonsoftSerializer>();

            newtonsoftSerializer.DeserializerSettings.ShouldBe(settings);
            newtonsoftSerializer.SerializerSettings.ShouldBe(settings);
        }

        [Fact]
        public void AddWithSettingsBuilderSuccessfully()
        {
            IApiConfiguration configuration = new ApiConfiguration();

            IApiConfigurationExtensions configurationExtensions = configuration.GetExtensions();

            Should.NotThrow(() => configurationExtensions.AddNewtonsoft(s => s.DateFormatString = "TEST"));

            IApiOptionsBuilder options = configuration.GetOptionsBuilder();

            using IDependencyProvider provider = options.Dependencies.BuildProvider();

            ISerializer serializer = provider.GetDependency<ISerializer>();

            NewtonsoftSerializer newtonsoftSerializer = serializer.ShouldBeOfType<NewtonsoftSerializer>();

            newtonsoftSerializer.DeserializerSettings.DateFormatString.ShouldBe("TEST");
            newtonsoftSerializer.SerializerSettings.DateFormatString.ShouldBe("TEST");
        }

        [Fact]
        public void ThrowArgumentNullException_Settings()
        {
            IApiConfiguration configuration = new ApiConfiguration();

            IApiConfigurationExtensions configurationExtensions = configuration.GetExtensions();

            JsonSerializerSettings settings = null;

            Should.Throw<ArgumentNullException>(() => configurationExtensions.AddNewtonsoft(settings));
        }

        [Fact]
        public void ThrowArgumentNullException_Builder()
        {
            IApiConfiguration configuration = new ApiConfiguration();

            IApiConfigurationExtensions configurationExtensions = configuration.GetExtensions();

            Action<JsonSerializerSettings> builder = null;

            Should.Throw<ArgumentNullException>(() => configurationExtensions.AddNewtonsoft(builder));
        }
    }
}
