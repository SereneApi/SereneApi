# Getting Started - AspNet
SereneApi is designed to work seamlessly and easily inside of your .Net5 AspNet application.

## Installing the Nuget Packages
First of you'll need to install the **SereneApi.AspNet** package from the Nuget pacakge manager. 

## Implementing your ApiHandler's
First off you'll need to setup your API definitions, this is done using an interface and a handler class. The interface defines the APIs methods in terms of what each method needs and what it supplies in return. Before we get started on that first you'll need to decide where these objects are going to be located within your project. Personally I do the following. Where folders are bolded and files are italicized.
* **MyAspNetProject**
	* **APIs**
		* **DTOs**
			* *BarDto.cs*
			* *FooDto.cs*
		* **Handlers**
			* *BarApiHandler.cs*
			* *FooApiHandler.cs*
		* *IBarApi.cs*
		* *IFooApi.cs*

Let's use the below ApiController to learn the basics of SereneApi.
``` csharp
[Route("api/[controller]")]
public class FooController : ControllerBase
{
	[HttpGet()]
	public Task<ActionResult<List<FooDto>>> GetAsync() ...
	
	[HttpGet("id")
	public Task<IActionResult<FooDto>> GetAsync(long id) ...

	[HttpPost()]
	public Task<IActionResult<FooDto>> CreateAsync(FooDto foo) ...

	[HttpDelete()]
	public Task<IActionResult<FooDto>> DeleteAsync(FooDto foo) ...
}
```
The following API resource has four endpoints. Two **Gets** one of them needing an id. And there is also a **Post** and **Delete** method, both of which require a object to be provided.

The first thing you need to do is create an API definition, this is done by creating an interface called *IFooApi*.
``` csharp
public interface IFooApi
{
	Task<IApiResponse<List<FooDto>> GetAsync();
	Task<IApiResponse<FooDto> GetAsync(long id);
	Task<IApiResponse<FooDto> CreateAsync(FooDto foo);
	Task<IApiResponse<FooDto> DeleteAsync(FooDto foo);
}
```
You've probably notice that each method returns an *IApiResponse* this interface encapsulates the response, it contains the the status code, content of the request and any data related to an exception.

The next step is setting the implementation class which is call an *ApiHandler*.

``` csharp
public class FooApiHandler: BaseApiHandler, IFooApi
{
	public FooApiHandler(IApiOptions<IFooApi> options): base(options)
	{
	}
	
	public Task<IApiResponse<List<FooDto>> GetAsync()
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.RespondsWithType<List<FooDto>>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<FooDto> GetAsync(long id)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithParameter(id)
			.RespondsWithType<FooDto>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<FooDto> CreateAsync(FooDto foo)
	{
		MakeRequest
			.UsingMethod(Method.Post)
			.AddInBodyContent(foo)
			.RespondsWithType<FooDto>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<FooDto> DeleteAsync(FooDto foo)
	{
		MakeRequest
			.UsingMethod(Method.Delete)
			.AddInBodyContent(foo)
			.RespondsWithType<FooDto>()
			.ExecuteAsync();
	}
}
```

## Registering your APIs in Startup.cs
``` csharp
public void ConfigureServices(IServiceCollection services)
{
	services.RegisterApi<IFooApi, FooApiHandler>()
}
```
## Consuming your APIs
``` csharp
public class MyClass
{
	private readonly IFooApi _fooApi;

	public MyClass(IFooApi fooApi)
	{
		_fooApi = fooApi
	}

	public async Task MyMethod(long id)
	{
		IApiResponse<FooDto> getFoo = await _fooApi.GetAsync(id);
		
		if(getFoo.WasNotSuccessful)
		{
			// Logic for handling failed request.
		}

		// Logic for handling successful request.
		FooDto foo = getFoo.Data;
	}
}
```