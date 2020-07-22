## Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Extensions.DependencyInjection**
## API Registration
To register an API, call the *RegisterApi* method inside the Startup.cs file. You are required to provide two types, the API's definition and the API's handler. The handler must inherit the definition.
*RegisterApi* has two parameters, one supplies the options builder and the adds an *IServiceProvider*
```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.RegisterApi<IFooApi, FooApiHandler>(b =>
	{
		b.UseConfiguration(Configuration.GetApiConfig("Foo");
	});
	
	services.RegisterApi<IBarApi, BarApiHandler>((b, p) =>
	{
		b.UseConfiguration(Configuration.GetApiConfig("Bar");
		// For demonstration purposes only. By default all APIs are configured to use ILoggerFactory.
		b.AddLogger(p.GetRequiredService<MyCustomLogger>());
	});
}
```
It is also possible to an extend an API's functionality. This can be done in two ways.

The first way is to extend it after registering the API.
```csharp
services.RegisterApi<IFooApi, FooApiHandler>(b =>
{
	b.UseConfiguration(Configuration.GetApiConfig("Foo");
})
.AddDIAuthenticator<IAuthenticationApi, TokenDto>(
    api => api.AuthenticateAsync(),
    dto => new TokenAuthResult(dto.AccessToken, dto.ExpiresIn));
```
The second way is to call the *ExtendApi* method.
>**NOTE:** *ExtendApi* does not necessary need to be called in Startup.cs, it can also be called elsewhere. A good example is inside your unit tests using *WebApplicationFactory\<TStartup>* alongside the *ConfigureTestServices* method override.
```csharp
builder.ConfigureTestServices(services =>
{
	services.ExtendApi<IFooApi>().WithMockResponse(r =>
	{
		r.AddMockResponse(new Foo())
			.ResponseToRequestsWith(Method.GET);
	});
});
```
## API Configuration using appsettings.json
It is possible to configure an API's connection information using *appsettings.json*. This is accomplished by adding an *ApiConfig* section in which all API configuration will be stored.

There are currently 5 settings that can be configured.
|Name|Optional?|Value|Default|Description|
|--|--|--|--|--|
|Source|**NO**|URI|null|The server source of the API.|
|Resource|Yes|string|null|The API resource.|
|ResourcePath|Yes|string|"api/"|The resource path.|
|Timeout|Yes|int|30|How many seconds before the request times out.|
|Retires|Yes|int|0|How many times the request will be re-attempted after it has timed out.|

>**NOTE:** It is not necessary to store API configuration under an *ApiConfig* section. But it is required if you wish to you *GetApiConfig("ApiName")* method.

### Example of appsettings.json
```json
"ApiConfig": {
    "Foo": {
        "Source": "http://www.somehost.com",
        "Resource": "Foo",
        "Retries": 2
    },
    "Bar": {
        "Source": "http://www.somehost.com",
        "Resource": "Bar",
        "ResourcePath": "api/Reporting/",
        "Timeout": 45
    }
}
```
To configure an API using *appsettings.json* call the *UseConfiguration* method whilst registering the API.
```csharp
services.RegisterApi<IFooApi, FooApiHandler>(b =>
{
	b.UseConfiguration(Configuration.GetApiConfig("Foo");
});
```

## Other
It is also possible to configure SerenaApi whilst using Dependency Injection. To do this call *ConfigureSereneApi* before registering any APIs. If it is called after API registration an exception will be thrown. In the below example, all APIs will navigate to api/v2/ and use the authentication API to authenticate.
```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.ConfigureSereneApi(b =>
	{
		b.ResourcePath = "api/v2/"
	}
	.AddDIAuthenticator<IAuthenticationApi, TokenDto>(
	    api => api.AuthenticateAsync(),
	    dto => new TokenAuthResult(dto.AccessToken, dto.ExpiresIn));
}
```