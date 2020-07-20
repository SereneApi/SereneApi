## Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Abstractions**
## Index
*	**[Authorization](#authorization)**
	*	**[Authorization Types](#authorization-types)**
	*	**[Authorizers](#authorizers)**
*	**[Configuration](#configuration)**
	*	**[Connection Settings](#connection-settings)**
	*	**[Default Configuration](#default-configuration)**
*	**[Factories](#factories)**
*	**[Handler](#handler)**
	*	**[IApiHandler](#iapihandler)**
	*	**[ICurdApi](#icrudapi)**
*	**[Options](#options)**
*	**[Queries](#queries)**
*	**[Request](#request)**
	*	**Content**
*	**[Response](#response)**
	*	**Content**
*	**[Routing](#routing)**
*	**[Serialization](#serialization)**
## Authorization
Contains all of the elements used in authorization. It is possible to implement the interfaces in this namespace to extend the functionality of authorization.

### IAuthorization
The *IAuthorization* interface is the primary method for authorization, excluding authentication using *ICredentials*. *IAuthorization* has two properties which are used to generate the 'authorization' header.
*	**Scheme** - Specifies the authorization scheme.
*	**Parameter** - Specifies the authorization parameter.

It is possible to implement a custom *IAuthorization* type to extend the authorization methods.
>**BEST PRACTICE:** All implementations of *IAuthorization* should follow the naming convention **Type**Authorization. Where type is *Bearer* or *Basic* etc.
### Authorization Types
Each type of authorization gets it's name from the method it uses to authorize. As an example *Bearer*Authorization authorizes with a bearer token.
*	**Basic Authorization**
	Authorizes using a *Username* and *Password* after converting the values into base64.
	>**NOTE:** This is not encrypted over HTTP.
*	**Bearer Authorization**
	Authorizes using a token.
	>**NOTE:** This is not encrypted over HTTP.
### Authorizers
*IAuthorizer* is different to *IAuthorization* due to the fact that  *IAuthorization* is used to set the 'authorization' header. Whereas *IAuthorizer* is solely a code construct used for runtime authorization. To implement *IAuthroizer* an *AuthorizeAsync* method must be implemented, which in turn returns *IAuthorization*.
```csharp
.AddAuthenticator<IAuthenticationApi, TokenDto>(
    api => api.AuthenticateAsync(),
    dto => new TokenAuthResult(dto.AccessToken, dto.ExpiresIn));
```
When *IAuthorization.AuthorizeAsync* is called in the example above it performs an API call using *IAuthenticationApi*. This returns a *TokenDto* which contains information used to generate a *BearerAuthorization* value. It also provides an expiry time that specifies how long until the token expires. *AuthenticateAsync* only calls the API if no token is present or the token has expired. This demonstrates the power of *IAuthorizer* as it can provided seamless back-end authorization with very little code required.
>**NOTE:** *IAuthorize.AuthorizeAsync* should only be called before authorizing or creating the HTTP request.
>
>**BEST PRACTICE:** When implementing *IAuthorize* it should only authorize when there is no authorization present or if the previous authorization has expired.
>This is due to the fact that authorizing every time could degrade application performance, especially if authorizing with an external API.
>
>**SEE:** *[TokenAuthorizer](https://github.com/armorall171/SereneApi/blob/master/src/SereneApi.Abstractions/Authorization/Authorizers/TokenAuthorizer.cs)* and *[InjectedTokenAuthorizer](https://github.com/armorall171/SereneApi/blob/master/src/SereneApi.Extensions.DependencyInjection/Authorizers/InjectedTokenAuthorizer.cs)* for more details on *IAuthorize* implementation. 
## Configuration
Contains all of the elements used for configuration. It is possible to implement the interfaces in this namespace to extend the functionality of configuration.
### Connection Settings
*IConnectionSettings* contains all of the information required for making requests against an API. 

*	**Base Address** | *Required*
	Specifies the hosts address.
	
*	**Resource** | *Optional*
	Specifies the resource that will be consumed.
	>**NOTE:** This value can be provided or overridden using the *AgainstResource* method when performing a request.
	
* **Resource Path** | *Optional*
	Specified the intermediate path between the base address and the resource.
	>**DEFAULT:** "api/";

* **Timeout** | *Optional*
	Specifies the amount of time in seconds the connection will be kept alive before being timed-out.
	>**DEFAULT:** 30 seconds;
	
* **Retry Attempts** | *Optional*
	Specifies how many times the connection will be retried after it has timed-out.
### Default Configuration
*IDefaultApiConfiguration* specifies default configuration for all APIs. The configuration is already configured out of the box, but it is possible to override these values. This allows global configuration and dependency extension to be applied easily. The following default configuration can be used.
*	**Resource Path**
*	**Timeout**
*	**Retry Attempts**
*	**Dependencies**
	*	**Add Dependency**
		> Adds a single dependency to the collection, this is handy for adding single new dependencies or overriding an existing.
	*	**Override Dependencies**
	This overrides all dependencies with the supplied dependencies.
		>**NOTE:** This will override all default dependencies. Use with *caution*.

These values can be overridden using the *ConfigureSereneApi* method.
```csharp
.ConfigureSereneApi(o =>
{
	o.ResourcePath = "api/v2/";
	o.AddDependnecy(d => d
		.AddScoped<ISerializer>(() => new MyCustomSerializer()));
});
```
## Factories
### IApiFactory
Implement this interface when creating an *ApiFactory*, this is only contained in abstractions to create a standardized interface model.
### IClientFactory
Used internally to build instances of *HttpClient* which are used to perform requests against an API. This method of *HttpClient* Instantiation is used by default. This is not a requirement if implementing a custom handler.
>**SEE:** *DefaultClientFactory* and *DefaultClientFactory\<TApi>* for more details on *IClientFactory* implementation.
## Handler
Contains abstrations for API handlers.
### IApiHandler
The *IApiHandler* interface needs to be implemented to use any of the *RegisterApi* methods. The interface is only required for the handler implementation.
### ICrudApi
When Inherited; provides the necessary methods for implementing a CRUD API consumer. This interface has two generic parameters that must be provided.
*	**TResource**
		Specifies the type that defines the APIs resource, this resource will be retrieved and provided by the API.
*	**TIdentifier**
		Specifies the identifier type used by the API to identify the resource.
	>**NOTE** Must be  struct type value.

The interface defines the following methods.
|Method|Parameter|Response|
|-|-|-
|GetAsync|TIdentifier|TResource|
|GetAsync||List\<TResource>|
|CreateAsync|TResource|TResource|
|DeleteAsync|TIdentifier||
|ReplaceAsync|TResource|TResource|
|UpdateAsync|TResource|TResource|

## Options
Contains abstractions and components used for creating and submitting 
API options. It is possible to override or extend functionality submitted to an API handler using this namespace.

### IApiOptions
### IApiOptionsBuilder
Defines the build options method. This interface exists to create separation between building and configuration of options. Both *IApiOptionsConfigurator* and *IApiOptionsBuilder* should both be inherited together.
>**SEE:** *[ApiOptionsBuilder](https://github.com/armorall171/SereneApi/blob/master/src/SereneApi.Abstractions/Options/ApiOptionsBuilder.cs)* for more details on implementation.
### IApiOptionsConfigurator
Defines methods for options configuration. This interface is intended to be used alongside the *RegisterApi* method. It will generate the configuration for *IApiOptions* before it is instantiated.

It is possible to extend configuration in several ways.

The first way it to implement your extension using the defined methods. Below a custom serializer is being provided using the *UseSerializer* method.
```csharp
public static void UseMySerializer(this IApiOptionsConfigurator configurator)
{
	configurator.UseSerializer(new MySerializer());
}
```
The second way is to get *ICoreOptions*, this gives access to the dependency collection allowing different dependencies to be added that are outside the scope of the methods defined by *IApiOptionsConfigurator*.
```csharp
public static void UseMySerializer(this IApiOptionsConfigurator configurator)
{
    if(!(extensions is ICoreOptions options))
    {
        throw new InvalidCastException($"Base type must inherit {nameof(ICoreOptions)}");
    }

	options.Dependencies.AddScoped<ISerializer>(() => new MySerializer());]
}
```
### IApiOptionsExtensions
Allows extensions of *ApiOptionsBuilder* post configuration. This interface does not define any methods or properties, but instead is intended to be used alongside *ICoreOptions*. This is intended to allow 3rd parties to easily add or extend functionality without the need to adjust or override the source.

When an extension method is written for this interface one needs to cast *IApiOptionsExtensions* to *ICoreOptions* to add or update dependencies.
```csharp
public static IApiOptionsExtensions MyOptionExtension(this IApiOptionsExtensions extensions)
{
    if(!(extensions is ICoreOptions options))
    {
        throw new InvalidCastException($"Base type must inherit {nameof(ICoreOptions)}");
    }

    options.Dependencies.AddScoped(() => // My extension);

    return extensions;
}
```
In the example above you can see how implementation should be done. The *IApiOptionsExtensions* is merely a wrapper for *ICoreOptions* and actual implementation of logic should be performed against *ICoreOptions*
## Queries
Contains abstractions and components used for queries.
### IQueryFactory
Converts the specified type into a query string before being appended to the end of a request. Only public properties are converted, it is possible to select specific public properties using a anonymous type lambda expression.

The query key will be based off of the property name unless otherwise overridden by *QueryKeyAttribute*.

The queries value is based off of a default delegate that is supplied in the constructor. A custom delegate can be provided, but it is recommended to use *QueryConverterAttribute*.
*	**RequiredAttribute**
		The specific property must be provided.
	>**NOTE:** *RequiredQueryElementException* will be thrown if the value was not provided.
*	**QueryKeyAttribute**
		The specific string value will be used as the key.
*	**QueryConverterAttribute**
	The specific converter will be used to convert the property value to a string. The converter must implement *QueryConverter* where the generic parameter is the type to be converted.
	>**NOTE:** It is possible to implement a custom converter using 
	*IQueryConverter*.
	
* **MinLengthAttribute**
	Specifies the minimum length of the query value. Applied after conversion, if value is not provided this will not be checked.
* **MaxLengthAttribute**
	Specifies the maximum length of the query value. Applies after conversion, if value is not provided this will not be checked.

### Attribute Examples
```csharp
public class Rfc3339QueryConverter: QueryConverter<DateTime>
{
    public override string Convert(DateTime value)
    {
        string valueString = value.ToString("yyyy-MM-ddThh:mm:ssZ");

        return valueString;
    }
}
```
```csharp
public class FooDto
{
	[Required]
	[QueryKey("foo_id")]
	public long Id{ get; set; }

	// Key will be 'FooDate'
	[QueryConverter(typeof(Rfc3339QueryConverter))]
	public DateTime FooDate{ get; set; }
}
```
## Request
## Response
## Routing
## Serialization