# SereneApi
SereneApi is intended to provide a straightforward way of consuming **RESTful** APIs requiring as little code & setup as possible whilst providing a powerful set of extensible tools.

### What does it do?
It consumes REST APIs whilst only needing several lines of code! That's it. Configuration is only done once during API registration and it seamlessly plugs into any AspNet application using DI.
### Why?
I found that creating Web Requests, then needing to Deserialize/Serialize usually became tedious and in some cases even broke DRY.

After using the repository patter for many years. I decided to try fiddling with the idea of an API consumer using a similar idea. After implementing a basic handler I decided the idea was found and began work on fleshing out SereneApi.
## How do I use it?
1.	Get it from nuget `Install-Package SereneApi.AspNet`
2.	Create your API definitions & implementations
3.	Register your APIs in startup.cs
4.	Consuming your APIs

### Creating your API Definitions & Implementations
First you'll need to create you API definition, this is accomplished by creating an interface like what is demonstrated below. Each method should return a Task<IApiResponse>
``` csharp
public interface IFooApi
{
	Task<IApiResponse<List<FooDto>> GetAsync();
	Task<IApiResponse<FooDto>> GetAsync(long id);
	Task<IApiResponse<FooDto>> CreateAsync(FooDto foo);
	Task<IApiResponse> DeleteAsync(FooDto foo);
}
```
Next you'll need to create your API implementation. This is done by creating an ApiHandler like what is demonstrated below.
``` csharp
public class FooApiHandler: BaseApiHandler, IFooApi
{
	public FooApiHandler(IApiOptions<IFooApi> options): base(options)
	{
	}
	
	public Task<IApiResponse<List<FooDto>>> GetAsync()
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.RespondsWithType<List<FooDto>>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<FooDto>> GetAsync(long id)
	{
		return MakeRequest
			.UsingMethod(Method.Get)
			.WithParameter(id)
			.RespondsWithType<FooDto>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse<FooDto>> CreateAsync(FooDto foo)
	{
		MakeRequest
			.UsingMethod(Method.Post)
			.AddInBodyContent(foo)
			.RespondsWithType<FooDto>()
			.ExecuteAsync();
	}
	
	public Task<IApiResponse> DeleteAsync(FooDto foo)
	{
		MakeRequest
			.UsingMethod(Method.Delete)
			.AddInBodyContent(foo)
			.ExecuteAsync();
	}
}
```

### Registering your APIs in Startup.cs
``` csharp
public void ConfigureServices(IServiceCollection services)
{
	services.RegisterApi<IFooApi, FooApiHandler>(o => 
	{
		o.AddConfiguration(Configuration.GetApiConfig("MyApi"));
	});
}
```
## Documentation
More in depth documentation is coming soon. Which will go into more advanced functionality and configuration.
## Consuming your APIs
``` csharp
public class MyService
{
	private readonly IFooApi _fooApi;

	public MyService(IFooApi fooApi)
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
## Stable Release
|Package|Downloads|Build|NuGet|
|-|-|-|-|
|**SereneApi**|![](https://img.shields.io/nuget/dt/SereneApi?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/14?style=for-the-badge)|[![Nuget](https://img.shields.io/nuget/v/SereneApi.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi/) |
|**SereneApi.AspNet**|![](https://img.shields.io/nuget/dt/SereneApi.AspNet?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/14?style=for-the-badge)|[![Nuget](https://img.shields.io/nuget/v/SereneApi.AspNet.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi.AspNet/) |
|**SereneApi.Abstractions**|![](https://img.shields.io/nuget/dt/SereneApi.Abstractions?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/13?style=for-the-badge)| [![Nuget](https://img.shields.io/nuget/v/SereneApi.Abstractions.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi.Abstractions/) |
|**SereneApi.Extensions.DependencyInjection**|![](https://img.shields.io/nuget/dt/SereneApi.Extensions.DependencyInjection?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/15?style=for-the-badge)|[![Nuget](https://img.shields.io/nuget/v/SereneApi.Extensions.DependencyInjection.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi.Extensions.DependencyInjection/)|
|**SereneApi.Extensions.Mocking**|![](https://img.shields.io/nuget/dt/SereneApi.Extensions.Mocking?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/16?style=for-the-badge)|[![Nuget](https://img.shields.io/nuget/v/SereneApi.Extensions.Mocking.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi.Extensions.Mocking/)|
|**SereneApi.Extensions.Newtonsoft**|![](https://img.shields.io/nuget/dt/SereneApi.Extensions.Newtonsoft?style=for-the-badge)|![Azure DevOps builds](https://img.shields.io/azure-devops/build/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/17?style=for-the-badge)|[![Nuget](https://img.shields.io/nuget/v/SereneApi.Extensions.Newtonsoft.svg?style=for-the-badge)](https://www.nuget.org/packages/SereneApi.Extensions.Newtonsoft/)|

## Tests
|Package||Coverage|
|-|-|-|
|**SereneApi**|![Azure DevOps tests](https://img.shields.io/azure-devops/tests/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/14?style=for-the-badge)|![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/14?style=for-the-badge)|
|**SereneApi.Abstractions**|![Azure DevOps tests](https://img.shields.io/azure-devops/tests/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/13?style=for-the-badge)|![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/13?style=for-the-badge)|
|**SereneApi.Extensions.DependencyInjection**|![Azure DevOps tests](https://img.shields.io/azure-devops/tests/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/15?style=for-the-badge)|![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/15?style=for-the-badge)|
|**SereneApi.Extensions.Mocking**|![Azure DevOps tests](https://img.shields.io/azure-devops/tests/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/16?style=for-the-badge)|![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/16?style=for-the-badge)|
|**SereneApi.Extensions.Newtonsoft**|![Azure DevOps tests](https://img.shields.io/azure-devops/tests/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/17?style=for-the-badge)|![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/DeltaWareAU/e18b43d4-35b6-4aa6-b09d-a50814de3303/17?style=for-the-badge)|







