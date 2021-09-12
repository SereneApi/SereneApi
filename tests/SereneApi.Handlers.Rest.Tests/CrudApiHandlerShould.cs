using SereneApi.Core.Handler.Factories;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Types;
using SereneApi.Extensions.Mocking.Rest;
using SereneApi.Handlers.Rest.Tests.Interfaces;
using SereneApi.Handlers.Rest.Tests.Mock;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SereneApi.Handlers.Rest.Tests
{
    public class CrudApiHandlerShould
    {
        private readonly ICrudApi<MockPersonDto, long> _crudApiHandler;

        public CrudApiHandlerShould()
        {
            ApiFactory factory = new ApiFactory();

            factory.RegisterApi<ICrudApi, CrudApiHandlerWrapper>(builder =>
            {
                builder.SetSource("http://localhost:8080", "Person");
            }).EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints("http://localhost:8080/api/Person/0")
                    .RespondsWith(MockPersonDto.BenJerry);

                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints("http://localhost:8080/api/Person/1")
                    .RespondsWith(MockPersonDto.JohnSmith);

                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .RespondsWith(MockPersonDto.All);

                c.RegisterMockResponse()
                    .ForMethod(Method.Delete)
                    .ForEndpoints("http://localhost:8080/api/Person/0")
                    .RespondsWith(Status.Ok);

                c.RegisterMockResponse()
                    .ForMethod(Method.Delete)
                    .ForEndpoints("http://localhost:8080/api/Person/2")
                    .RespondsWith(new FailureResponse("Could not find a Person with an Id of 2"), Status.NotFound);

                c.RegisterMockResponse()
                    .ForMethod(Method.Post)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .ForContent(MockPersonDto.BenJerry)
                    .RespondsWith(MockPersonDto.BenJerry);

                c.RegisterMockResponse()
                    .ForMethod(Method.Post)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .ForContent(MockPersonDto.JohnSmith)
                    .RespondsWith(new FailureResponse("This person has already been added."), Status.BadRequest);

                c.RegisterMockResponse()
                    .ForMethod(Method.Put)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .ForContent(MockPersonDto.BenJerry)
                    .RespondsWith(MockPersonDto.BenJerry);

                c.RegisterMockResponse()
                    .ForMethod(Method.Patch)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .ForContent(MockPersonDto.BenJerry)
                    .RespondsWith(MockPersonDto.BenJerry);

                c.RegisterMockResponse()
                    .ForMethod(Method.Patch)
                    .ForEndpoints("http://localhost:8080/api/Person")
                    .ForContent(MockPersonDto.JohnSmith)
                    .RespondsWith(new FailureResponse("Could not find the specified user"), Status.NotFound);
            });

            _crudApiHandler = factory.Build<ICrudApi>();
        }

        [Fact]
        public async Task Create()
        {
            IApiResponse<MockPersonDto> response = await _crudApiHandler.CreateAsync(MockPersonDto.BenJerry);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();

            MockPersonDto person = MockPersonDto.BenJerry;

            person.BirthDate.ShouldBe(MockPersonDto.BenJerry.BirthDate);
            person.Age.ShouldBe(MockPersonDto.BenJerry.Age);
            person.Name.ShouldBe(MockPersonDto.BenJerry.Name);

            response = await _crudApiHandler.CreateAsync(MockPersonDto.JohnSmith);

            response.WasSuccessful.ShouldBe(false);
            response.Status.ShouldBe(Status.BadRequest);
            response.Message.ShouldBe("This person has already been added.");
            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);
        }

        [Fact]
        public async Task Delete()
        {
            IApiResponse response = await _crudApiHandler.DeleteAsync(0);

            response.WasSuccessful.ShouldBe(true);
            response.Status.ShouldBe(Status.Ok);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);

            response = await _crudApiHandler.DeleteAsync(2);

            response.WasSuccessful.ShouldBe(false);
            response.Status.ShouldBe(Status.NotFound);
            response.Message.ShouldBe("Could not find a Person with an Id of 2");
            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);
        }

        [Fact]
        public async Task Get()
        {
            IApiResponse<MockPersonDto> response = await _crudApiHandler.GetAsync(0);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();

            MockPersonDto person = MockPersonDto.BenJerry;

            person.BirthDate.ShouldBe(MockPersonDto.BenJerry.BirthDate);
            person.Age.ShouldBe(MockPersonDto.BenJerry.Age);
            person.Name.ShouldBe(MockPersonDto.BenJerry.Name);

            response = await _crudApiHandler.GetAsync(1);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();

            person = MockPersonDto.JohnSmith;

            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
        }

        [Fact]
        public async Task GetAll()
        {
            IApiResponse<List<MockPersonDto>> response = await _crudApiHandler.GetAsync();

            response.Data.Count.ShouldBe(MockPersonDto.All.Count);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.WasSuccessful.ShouldBe(true);

            List<MockPersonDto> results = response.Data;

            for (int i = 0; i < results.Count; i++)
            {
                results[i].Name.ShouldBe(MockPersonDto.All[i].Name);
                results[i].Age.ShouldBe(MockPersonDto.All[i].Age);
                results[i].BirthDate.ShouldBe(MockPersonDto.All[i].BirthDate);
            }
        }

        [Fact]
        public async Task Replace()
        {
            IApiResponse<MockPersonDto> response = await _crudApiHandler.ReplaceAsync(MockPersonDto.BenJerry);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();

            MockPersonDto person = MockPersonDto.BenJerry;

            person.BirthDate.ShouldBe(MockPersonDto.BenJerry.BirthDate);
            person.Age.ShouldBe(MockPersonDto.BenJerry.Age);
            person.Name.ShouldBe(MockPersonDto.BenJerry.Name);
        }

        [Fact]
        public async Task Update()
        {
            IApiResponse<MockPersonDto> response = await _crudApiHandler.UpdateAsync(MockPersonDto.BenJerry);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();

            MockPersonDto person = MockPersonDto.BenJerry;

            person.BirthDate.ShouldBe(MockPersonDto.BenJerry.BirthDate);
            person.Age.ShouldBe(MockPersonDto.BenJerry.Age);
            person.Name.ShouldBe(MockPersonDto.BenJerry.Name);

            response = await _crudApiHandler.UpdateAsync(MockPersonDto.JohnSmith);

            response.WasSuccessful.ShouldBe(false);
            response.Status.ShouldBe(Status.NotFound);
            response.Message.ShouldBe("Could not find the specified user");
            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);
        }
    }
}