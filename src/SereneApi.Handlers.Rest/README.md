# RESTful Api Handler
The RestApiHandler allows SereneApi to communicate with RESTful Apis. The handler's Fluent Api and configuration allows you to fully and easily integrate with and consume a RESTful Api. The following guide will cover the following aspects of the RestApiHandler
*	Configuration
*	Performing a Request
*	Sending and Receiving in body content.
*	Queries
*	Parameters

### Getting Started

Firstly you'll need to install the latest version of the nuget package ```SereneApi.Handlers.Rest```

## Creating a RestApiHandler
The ApiHandler patter was inspired by EF Core and the Repository model. The example will be based on a RESTful API for Students and will cover the basics of making requests.

#### Implementing the Definition

To begin with you'll need to create the definition, the definition should use the following naming convention ```I{Resource}Api```. If the resource is **Student** then the name would be ```IStudentApi```.

```csharp
public interface IStudentApi
{
	Task<IApiResponse<List<StudentDto>>> GetAsync();
	
	Task<IApiResponse<StudentDto>> GetAsync(long studentId);

	Task<IApiResponse<List<StudentDto>>> GetByClassAsync(long classId);

	Task<IApiResponse<List<StudentDto>>> SearchAsync(string firstName, string lastName, DateTime birthDate);

	Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto searchStudent);

	Task<IApiResponse<StudentDto>> CreateAsync(StudentDto newStudent);

	Task<IApiResponse> DeleteAsync(long studentId);
}
```
#### Implementing the Handler
Now that we've finished defining the Api we need to create the handler for the Api. The handler should use the following naming convention ```{Resource}ApiHandler```. If the resource is **Student** then the name would be ```StudentApiHandler```.

```csharp
public class StudentApiHandler: RestApiHandler, IStudentApi
{
	public StudentApiHandler(IApiSettings<StudentApiHandler> setting): base(settings)
	{
	}
	
	public Task<IApiResponse<List<StudentDto>>> GetAsync()
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<StudentDto>> GetAsync(long studentId)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithParameter(studentId)
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync();
	}

	public Task<IApiResponse<List<StudentDto>>> GetByClassAsync(long classId)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.AgainstEndpoint("ByClass/{0}")
			.WithParameter(classId)
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync();
	}

	public Task<IApiResponse<List<StudentDto>>> SearchAsync(string firstName, string lastName, DateTime birthDate)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithEndpoint("Search")
			.WithQuery(new 
			{
				firstName,
				lastName,
				birthDate
			})
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync():
	}

	public Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto searchStudent)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithEndpoint("Search")
			.WithQuery(searchStudent, s => new 
			{ 
				s.FirstName, 
				s.LastName,
				s.BirthDate
			})
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync():
	}

	public Task<IApiResponse<StudentDto>> CreateAsync(StudentDto newStudent)
	{
		return MakeRequest
			.UsingMethod(Method.Post)
			.AddInBodyContent(newStudent)
			.RespondsWith<StudentDto>()
			.ExecuteAsync();
	}

	public Task<IApiResponse> DeleteAsync(long studentId)
	{
		return MakeRequest
			.UsingMethod(Method.Delete)
			.AddParameter(studentId)
			.ExecuteAsync();
	}
}
```

As you can see in the above example configuring your requests is rather straightforward thanks to the use of a Linq like Fluent API.

## MakeRequest Fluent API
> **NOTE**: The order outlined below is the order that they must executed in.
#### UsingMethod
This method specifies the HttpMethod to be used for the request.
#### AgainstResource
This method allows the resource to either be specified or overridden.
#### AgainstVersion
This method allows the version to be specified.
#### AgainstEndpoint
This methods allows the endpoint to be specified. It also supports formatting to be used alongside the ```WithParameter``` method.
#### WithParameter
This method allows parameters to be appended to the endpoint of a request.
#### WithQuery
Appends a query to the Url of the request. It is appended after the endpoint. All of the formatting is handled by the ```IQuerySerializer```.

Below are three example of the different overloads available.

```csharp
.WithQuery(student)
// -- OR --
.WithQuery(new 
{
	firstName,
	lastName,
	birthDate
})
```
The above overload will use all public properties to build the query. The object provided can either be a type or an anonymous type.

> **NOTE**: Empty or null properties will be ignored.

```csharp
.WithQuery(student, s => new
{
	s.FirstName,
	s.LastName,
	s.BirthDate
})
```
The above overload will only used the selected properties from the specified type.
```csharp
.WithQuery<StudentDto>(s => 
{
	s.FirstName = firstName,
	s.LastName = lastName,
	s.BirthDate = birthDate
})
```
The above example allows you to instantiate the specified type to be used for the query.

> **NOTE**: Empty or null properties will be ignored.

#### AddInBodyContent
Serializes and adds content to the body of the request. By default the content is JSON.
#### RespondsWith
Specifies that the handler should deserialize the response content to the specified type.
#### ExecuteAsync
Executes the request asynchronously returning an ```IApiResponse``` when the execution has concluded.

### Skipping the IApiResponse
#### ThrowOnFail
Throws an ```UnsuccessfulResponseException``` if the status of a response is not a 200.

```csharp
public Task<IApiResponse<List<StudentDto>>> GetAsync()
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.RespondsWith<List<StudentDto>>()
		.ExecuteAsync()
		.ThrownOnFail();
}
```

#### GetDataAsync
Unwraps the data from an ```IApiResponse```.

```csharp
public Task<List<StudentDto>> GetAsync()
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.RespondsWith<List<StudentDto>>()
		.ExecuteAsync()
		.ThrownOnFail()
		.GetDataAsync();
}
```

#### GetStatusAsync
Unwraps the status from an ```IApiResponse```.

```csharp
public Task<Status> DeleteAsync(long studentId)
{
	return MakeRequest
		.UsingMethod(Method.Delete)
		.AddParameter(studentId)
		.ExecuteAsync()
		.GetStatusAsync();
}
```

## CrudApiHandler
The CrudApiHandler implements all of the basic Crud operations, usage is exactly the same as the RestApiHandler. 

> **NOTE**: Details on what operations are implemented by the CrudApiHandler are listed below the implementation examples.

#### Implementing the Definition
```csharp
public interface IStudentApi: ICrudApi<StudentDto, long>
{
	Task<IApiResponse<List<StudentDto>>> GetByClassAsync(long classId);

	Task<IApiResponse<List<StudentDto>>> SearchAsync(string firstName, string lastName, DateTime birthDate);

	Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto searchStudent);
}
```
#### Implementing the Handler
```csharp
public class StudentApiHandler: CrudApiHandler<StudentDto, long>, IStudentApi
{
	public StudentApiHandler(IApiSettings<StudentApiHandler> setting): base(settings)
	{
	}
	
	public Task<IApiResponse<List<StudentDto>>> GetByClassAsync(long classId)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.AgainstEndpoint("ByClass/{0}")
			.WithParameter(classId)
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync();
	}

	public Task<IApiResponse<List<StudentDto>>> SearchAsync(string firstName, string lastName, DateTime birthDate)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithEndpoint("Search")
			.WithQuery(new 
			{
				firstName,
				lastName,
				birthDate
			})
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync():
	}

	public Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto searchStudent)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithEndpoint("Search")
			.WithQuery(searchStudent, s => new 
			{ 
				s.FirstName, 
				s.LastName,
				s.BirthDate
			})
			.RespondsWith<List<StudentDto>>()
			.ExecuteAsync():
	}
}
```
#### CrudApi Definition
The CrudApiHandler implements the following Crud operations.
```csharp
interface ICrudApi<TResource, in TIdentifier> where TResource : class where TIdentifier : struct
{
	Task<IApiResponse<TResource>> CreateAsync(TResource resource);
	Task<IApiResponse> DeleteAsync(TIdentifier identifier);
	Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier);
	Task<IApiResponse<List<TResource>>> GetAsync();
	Task<IApiResponse<TResource>> ReplaceAsync(TResource resource);
	Task<IApiResponse<TResource>> UpdateAsync(TResource resource);
}
```

## Configuration

Configuration in SereneApi is extensive, so I won't cover the advanced area. But to give a rough overview, internall SereneApi is using Dependency Injection to provide all of the dependencies for the Handler.

#### Handler Configuration Provider

All of the handlers configuration is stored in the ```RestHandlerConfigurationProvider``` it is possible to override the configuration provider to provide different configuration. To override the configuration provider simple do the following.

Below is a custom implementation of a Configuration Provider, it is important to inherit from the ```RestHandlerConfigurationProvider``` so all of the required configuration is implemented. In the constructor of the custom configuration provider we can override all of the configuration.
```csharp
public class FooHanderConfigurationProvider: RestHandlerConfigurationProvider
{
	public FooHanderConfigurationProvider()
	{
		Dependencies.AddScoped<ISerializer, MySerializer>();
	}
}
```
Below we're using the ```UseHandlerConfigurationProviderAttribute``` to inform SereneApi that we want to use that configuration provider instead of the default for the Api Handler.
```csharp
[UseHandlerConfigurationProvider(typeof(FooHanderConfigurationProvider))]
public class FooApiHandler, RestApiHandler, IFooApi
{
}
```

## Registration
API registering is important as it not only binds the API to the handler but it also allows configuration to be provided. API Registering can currently be done using one of two methods.
### ApiFactory

```csharp
ApiFactory factory = new ApiFactory();

factory.RegisterApi<IFooApi, FooApiHandler>(c => 
{
	c.SetSource("http://localhost", "Foo");
});
```

### AspNet Dependency Injection

```csharp
services.RegisterApi<IFooApi, FooApiHandler(c =>
{
	c.SetSource("http://localhost", "Foo");
	// -- OR --
	c.AddConfiguration(Configuration);
});
```

## API Invocation

### Invoking with ApiFactory
Invocation with *ApiFactory* can be done with either the class or the interface, in the example below the interface will be used because it does not expose the registration methods.
When an API is required, call the ```BuildApi<TApi>()``` method. This provides an instantiated instance of TApi.
>**NOTE:** The instance of TApi needs to implement IDisposable. 
```csharp
public class FooService
{
	private readonly IApiFactory _factory;

	public void DoStuff(long id)
	{
		IApiResponse<FooDto> response;

		using (IFooApi fooApi = _factory.Build<IFooApi>())
		{
			response = fooApi.GetAsync(id);
		}

		// Do stuff on response here.
	}
}
```
### Invoking with Dependency Injection
Invocation with Dependency Injection is easy and straightforward. Add your APIs implementation interface to the constructor of your class and DI will handle the rest.
>**NOTE:** The API should not be disposed of as this is handled by dependency injection.
```csharp
public class FooService
{
	private readonly IFooApi _fooApi;

	public FooService(IFooService fooApi)
	{
		_fooApi = fooApi;
	}

	public void DoStuff(long id)
	{
		IApiResponse<FooDto> response = fooApi.GetAsync(id);
		
		// Do stuff on response here.
	}
}
```

