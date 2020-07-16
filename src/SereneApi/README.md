# Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi**
## Index
*	**[Implementation](#api-implementation)**
*	**[Registration](#api-registration)**
*	**[Instantiation](#api-instantiation)**
*	**[ApiHandler](#apihandler)**
*	**[Other](#other)**
## API Implementation
The first step in implementing an API, is its definition. The definition is an interface representing all available actions that can be performed against an API.

Below is an example of an API intended for the resource 'Foo'. The API contains two actions, A GET that requires an ID and a CREATE that requires a *FooDto* object.
>**BEST PRACTICE:** API methods should return an *IApiResponse*.
```csharp
public interface IFooApi: IDisposable
{
	Task<IApiResposne<FooDto>> GetAsync(long id);

	Task<IApiResponse> CreateAsync(FooDto foo);
}
```
After an API's definition has been implemented, its associated ApiHandler needs to be created. The handler is the backbone of an API, performing all of the magic required for sending and receiving requests.
>**BEST PRACTICE:** ApiHandler implementations should be located in a *Handlers* folder contained in your project.


```csharp
public class FooApiHandler: ApiHandler, IFooApi
{
	public FooApiHandler(IApiOptions<IFooApi> options) : base(options)
	{
	}
	
	public Task<IApiResposne<FooDto>> GetAsync(long id)
	{
		return PerformRequestAsync<FooDto>(Method.GET, r => r
			.WithEndPoint(id));
	}
	
	public Task<IApiResonse> CreateAsync(FooDto foo)
	{
		return PerformRequestAsync(Method.POST, r => r
			.AddInBodyContent(foo));
	}
}
```
There are a couple of things to take note of in the above example.
*	*FooApiHandler* inherits the abstract class *ApiHandler*.
*	*FooApiHandler* inherits the interface *IFooApi*.
*	*FooApiHandler's* constructor contains a single parameter *IApiOptions* with the generic type set to *IFooApi*.

Once an API's definition and handler have been implemented it is now possible to register them.
## API Registration
API registering is important as it not only binds the API to the handler but it also allows configuration to be provided. API Registering can currently be done using one of two methods.

### ApiFactory Method
```csharp
ApiFactory factory = new ApiFactory();

factory.RegisterApi<IFooApi, FooApiHandler>(o => 
{
	o.UseSource("http://www.somehost.com", "Foo");
	o.UseLogger(myLogger);
});
```
### Dependency Injection Method
>**NOTE:** To use dependency injection  install *SereneApi.Extensions.DependencyInjection*

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.RegisterApi<IFooApi, FooApiHandler>(b =>
	{
		o.UseSource("http://www.somehost.com", "Foo");
	});
}
```
Both of the above registrations will deliver the same configuration for Foo API.
>**NOTE:** By default all APIs registered using dependency injection will attempt to get an *ILogger* using *ILoggerFactory*.
## API Instantiation
After an API has been registered it's handler needs to be instantiated, this is where the *IApiOptions* parameter comes in. *IApiOptions* contains all of the API's configuration and dependencies as declared during registration. It is important to set your API definition as the generic type for *IApiOptions*.
>**NOTE:** If an API's handler does not have *IApiOptions* configured correctly, it can either get the settings for a different handler or an exception may be thrown.

This process occurs behind the scenes but it still needs to be invoked.

### Invoking with ApiFactory
Invocation with *ApiFactory* can be done with either the class or the interface, in the example below the interface will be used because it does not expose the registration methods.
When an API is required, call the *Build\<TApi>()* method. This provides an instantiated instance of TApi.
>**NOTE:** The instance of TApi needs to be disposed.
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
Invocation with Dependency Injection is easy and straightforward. Add your API's implementation interface to the constructor of your class and DI will handle the rest.
>**NOTE:** The API should not be disposed of as this is handled by DI.
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

## ApiHandler
Inheriting the base class *ApiHandler* gives access to several protected methods for performing requests.
```csharp
PerformRequest();
PerformRequestAsync();
PerformRequest<TResponse>();
PerformRequestAsync<TResponse>();
```
Each of the methods listed above share the same parameters. The first parameter is the *Method* in which the request will be performed. The methods are the standard array of REST methods.
* **POST** - *Submits an entity to the specified resource.*
* **GET** - *Requests a representation of the specified resource.*
* **PUT** - *Replaces all current representations of the target resource.*
* **PATCH** - *Applies partial modifications to a resource.*
* **DELETE** - *Deletes the specified resource.*

The second parameter is the request factory, this parameter is entirely optional. It contains multiple methods that are required to be called in a specific order.
* **AgainstResource**
	> Specifies or overrides the resource that the request is intended for. If the resource was provided during configuration this method is not necessary.
* **WithEndpoint**
	>Specifies the endpoint for the request, it is applied after the resource.
* **WithEndpointTemplate**
	>Specifies the endpoint for the request, is is applied after the resource. Two parameters must be provided, a format-table string and an array of objects that will be formatted to the string.
* **WithInBodyContent\<TContent>**
	>Specifies the content to be sent in the body of the request.
	>
	>**NOTE**: This method can only be used in conjunction with *POST*, *PUT* and *PATCH*.
* **WithQuery**
	>Specifies the query to be added to the end of the requests Uri.
	## Other
	A variant of *PerformRequest* is available. This method takes an *IApiRequest* as its sole parameter, enabling pre-configured or custom requests to be used instead of the request factory.