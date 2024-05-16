using FluentAssertions;
using SereneApi.Request.Attributes;
using SereneApi.Request.Attributes.Parameter;
using SereneApi.Request.Attributes.Request;
using SereneApi.Resource;
using SereneApi.Resource.Exceptions;

namespace SereneApi.Tests.Resource
{
    public class ApiResourceRegistryShould
    {
        [HttpResource]
        interface IMultipleContentParametersApi
        {
            [HttpPostRequest]
            Task CreateAsync([HttpContent] object a, [HttpContent] object b);
        }

        [Fact]
        public void Throw_When_MultipleContentParametersFound()
        {
            Action act = () => new ApiResourceProvider(new SpecificTypeResourceCollection<IMultipleContentParametersApi>());

            act.Should()
                .Throw<InvalidResourceSchemaException>()
                .WithMessage("The Method CreateAsync contains multiple content parameters [a,b], no more than 1 can be defined at a time.");
        }

        [HttpResource]
        interface IUnmappedMethodParametersApi
        {
            [HttpGetRequest("{id}")]
            Task GetAsync(Guid id, string unmapped);
        }

        [Fact]
        public void Throw_When_MethodHasUnmappedMethodParameters()
        {
            Action act = () => new ApiResourceProvider(new SpecificTypeResourceCollection<IUnmappedMethodParametersApi>());

            act.Should().Throw<InvalidResourceSchemaException>()
                .WithMessage("The Method GetAsync contains Method Parameters that do not map to the Endpoint Template [unmapped]");
        }

        [HttpResource]
        interface IUnmappedTemplateParametersApi
        {
            [HttpGetRequest("{id}/{unmapped}")]
            Task GetAsync(Guid id);
        }

        [Fact]
        public void Throw_When_MethodHasUnmappedTemplateParameters()
        {
            Action act = () => new ApiResourceProvider(new SpecificTypeResourceCollection<IUnmappedTemplateParametersApi>());

            act.Should().Throw<InvalidResourceSchemaException>()
                .WithMessage("The Method GetAsync Specifies Parameters in its Template that do not map to the Method Parameters [unmapped]");
        }
    }
}