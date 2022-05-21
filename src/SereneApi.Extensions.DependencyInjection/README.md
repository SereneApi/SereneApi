# AspNet Dependency Injection
This package enables an API to added to ```Microsoft.Extenesion.DependencyInjection``` enabling dependency injection in AspNet and the APIs lifetime to be handled by AspNet.

## Usage

Firstly you'll need to install the latest version of the nuget package ```SereneApi.Extensions.DependencyInjection```

After the nuget package has been installed the following ServiceCollection method extensions become available.

**AmendConfigurationProvider\<TConfigurationProvider>**
Allows the configuration provided to be amended.

**RegisterApi\<TApi, TApiHandler>**
Registers an API to Dependecy Injection.

**ExtendApi\<TApiHandler>**
Allows a previously registered API handler to amended.

## AspNet Configuration

Handler configuration can be completed using ```appsettings.json```, to use AspNet configuration the following method should be called during registration.
```csharp
services.RegisterApi<IFooApi, FooApiHandler>(c => 
{
	c.AddConfigruation(Configuration, "FooApi");

	-- or --

	c.AddConfigruation(Configuration, "FooApi", "SomeHostUrl");
}):
``` 
### appsettings.json Parameters
The below table outlines the parameters that can be used in AspNet Configuration.

|Name|Optional?|Value|Default|Description|
|--|--|--|--|--|
|Source|**NO**|URI|null|The server source of the API.|
|Resource|Yes|string|null|The API resource.|
|ResourcePath|Yes|string|"api/"|The resource path.|
|Timeout|Yes|int|30|How many seconds before the request times out.|
|Retires|Yes|int|0|How many times the request will be re-attempted after it has timed out.|

### Example appsettings.json
The below example outlines how Api configuration is done. Note how all of the config is contained withing the ```ApiConfig``` section, this is required.
```json
{
	"ApiConfig": {
	    "FooApi": {
	        "Source": "http://www.somehost.com",
	        "Resource": "Foo",
	        "Retries": 2
	    },
	    "BarApi": {
	        "Source": "http://www.somehost.com",
	        "Resource": "Bar",
	        "ResourcePath": "api/Reporting/",
	        "Timeout": 45
	    }
	}
}
```
The next example shows how the Source can be configured differently, this may be done so the same Url doesn't need to be repeated multiple times.
```json
{
	"ApiConfig": {
		"SomeHostUrl": "http://www.somehost.com",
		"FooApi": {
			"Resource": "Foo",
			"Retries": 2
		},
		"BarApi": {
			"Resource": "Bar",
			"ResourcePath": "api/Reporting/",
			"Timeout": 45
		}
	}
}
```

### GetApiConfig
There is also an extension method added for ```IConfiguration``` this method allows an ```IConnectionSettings``` to be created from AspNet Configuration.

```csharp
IConnectionSettings connection;

connection = Configuration.GetApiConfig("FooApi");

-- OR --

connection = Configuration.GetApiConfig("FooApi", "SomeHostUrl");
```


