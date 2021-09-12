using DeltaWare.Dependencies.Abstractions;
using Newtonsoft.Json;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Connection;
using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;
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
            ConfigurationManager configuration = new();

            Should.NotThrow(() => configuration.AmendConfiguration<TestFactory>(c =>
            {
                c.AddNewtonsoft();
            }));

            ApiOptionsFactory<TestHandler> optionsFactory = configuration.BuildApiOptionsFactory<TestHandler>();

            IDependencyProvider dependencies = optionsFactory.Dependencies.BuildProvider();

            ISerializer serializer = dependencies.GetDependency<ISerializer>();

            serializer.ShouldBeOfType<NewtonsoftSerializer>();
        }

        [Fact]
        public void AddWithSettingsBuilderSuccessfully()
        {
            string dateFormat = "TEST";

            ConfigurationManager configuration = new();

            Should.NotThrow(() => configuration.AmendConfiguration<TestFactory>(c =>
            {
                c.AddNewtonsoft(s => s.DateFormatString = dateFormat);
            }));

            ApiOptionsFactory<TestHandler> optionsFactory = configuration.BuildApiOptionsFactory<TestHandler>();

            IDependencyProvider dependencies = optionsFactory.Dependencies.BuildProvider();

            ISerializer serializer = dependencies.GetDependency<ISerializer>();

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

            ConfigurationManager configuration = new();

            Should.NotThrow(() => configuration.AmendConfiguration<TestFactory>(c =>
            {
                c.AddNewtonsoft(settings);
            }));

            ApiOptionsFactory<TestHandler> optionsFactory = configuration.BuildApiOptionsFactory<TestHandler>();

            IDependencyProvider dependencies = optionsFactory.Dependencies.BuildProvider();

            ISerializer serializer = dependencies.GetDependency<ISerializer>();

            NewtonsoftSerializer newtonsoftSerializer = serializer.ShouldBeOfType<NewtonsoftSerializer>();

            newtonsoftSerializer.DeserializerSettings.ShouldBe(settings);
            newtonsoftSerializer.SerializerSettings.ShouldBe(settings);
        }

        [Fact]
        public void ThrowArgumentNullException_Builder()
        {
            ConfigurationManager configuration = new();

            Action<JsonSerializerSettings> builder = null;

            Should.Throw<ArgumentNullException>(() => configuration.AmendConfiguration<TestFactory>(c =>
            {
                c.AddNewtonsoft(builder);
            }));
        }

        [Fact]
        public void ThrowArgumentNullException_Settings()
        {
            ConfigurationManager configuration = new();

            JsonSerializerSettings settings = null;

            Should.Throw<ArgumentNullException>(() => configuration.AmendConfiguration<TestFactory>(c =>
            {
                c.AddNewtonsoft(settings);
            }));
        }

        public class TestFactory : ConfigurationFactory
        {
        }

        [UseConfigurationFactory(typeof(TestFactory))]
        public class TestHandler : IApiHandler
        {
            public IConnectionSettings Connection => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}