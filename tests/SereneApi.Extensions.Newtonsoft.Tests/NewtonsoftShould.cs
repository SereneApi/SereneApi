using DeltaWare.Dependencies;
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
            ISereneApiConfiguration configuration = new SereneApiConfiguration();

            ISereneApiExtensions extensions = configuration.GetExtensions();

            Should.NotThrow(() => extensions.AddNewtonsoft());

            IApiOptionsBuilder options = configuration.GetOptionsBuilder();

            using IDependencyProvider provider = options.Dependencies.BuildProvider();

            ISerializer serializer = provider.GetDependency<ISerializer>();

            serializer.ShouldBeOfType<NewtonsoftSerializer>();
        }

        [Fact]
        public void AddWithSettingsSuccessfully()
        {
            ISereneApiConfiguration configuration = new SereneApiConfiguration();

            ISereneApiExtensions extensions = configuration.GetExtensions();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = "TEST"
            };

            Should.NotThrow(() => extensions.AddNewtonsoft(settings));

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
            ISereneApiConfiguration configuration = new SereneApiConfiguration();

            ISereneApiExtensions extensions = configuration.GetExtensions();

            Should.NotThrow(() => extensions.AddNewtonsoft(s => s.DateFormatString = "TEST"));

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
            ISereneApiConfiguration configuration = new SereneApiConfiguration();

            ISereneApiExtensions extensions = configuration.GetExtensions();

            JsonSerializerSettings settings = null;

            Should.Throw<ArgumentNullException>(() => extensions.AddNewtonsoft(settings));
        }

        [Fact]
        public void ThrowArgumentNullException_Builder()
        {
            ISereneApiConfiguration configuration = new SereneApiConfiguration();

            ISereneApiExtensions extensions = configuration.GetExtensions();

            Action<JsonSerializerSettings> builder = null;

            Should.Throw<ArgumentNullException>(() => extensions.AddNewtonsoft(builder));
        }
    }
}
