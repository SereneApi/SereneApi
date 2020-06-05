# Getting Started
First off, an ApiHandler will need to be implemented. In this example the **User** resource will be consumed. The naming convention can be seen below.
* Api Definition: I**Resource**Api
>**Best Practices Tip:** ApiHandler Implementations should be located in a Handlers folder somewhere in the project.

**Implementing the UserApiHandler**
```csharp
public class UserApiHandler : ApiHandler
{
	public UserApiHandler(IApiHandlerOptions options) : base(options)
	{
	}
	
	public Task<IApiResposne<UserDto>> GetAsync(long userId)
	{
		return InPathRequestAsync<UserDto>(Method.Get, userId);
	}
	
	public Task<IApiResonse<UserDto>> CreateAsync(UserDto user)
	{
		return InBodyRequestAsync<UserDto, UserDto>(Method.Post, user)
	}
}
```
After the Api **Definition**[*interface*] has been created, it will need to be configured using the **ApiHandlerFactory**. The Factory configures, creates and handles the lifetime of its Handlers.

**Using the ApiHandlerFactory**
```csharp
using (ApiHandlerFactory handlerFactory = new ApiHandlerFactory())
{
    handlerFactory.AddApiHandler<UserApiHandler>(o => 
    { 
	    o.UseSource("http://localhost:8080", "User");
	    o.AddLogger(new Logger());
    });

    using (UserApiHandler userApi = handlerFactory.Build<UserApiHandler>())
    {
        IApiResponse<UserDto> userResponse = await userApi.GetAsync(42);

        if (userResponse.WasSuccessful)
        {
            return userResponse.Result;
        }
    }
}
```
In the example above the UserApiHandler is being registered with the ApiHandlerFactory. Both the Source and Logger are being configured.

After the Handler has been registered the Build method can be called. Which will create an instance of the specified handler type.
> **NOTE:** The ApiHandlerFactory does not necessarily need to be immediately disposed of and can be configured elsewhere in the project.

## ApiHandler

The **ApiHandler** is a core a part of **SereneApi** and handles every single request. When inherited it offers a plethora of simple to use and yet powerful methods that should cover most, if not all use cases and is completely extensible. It also containing an extensive suite of logging. Covering all exceptions and messages.
>**Take Note:** No examples will be provided in this chapter. Review the Examples section to see it in action.
### Methods
The Method Enum informs the ApiHandler what **RESTful** method will be used for the request. Below are the methods made available.
```csharp
public enum Method
{
	Post,
	Get,
	Put,
	Patch,
	Delete
}
```
### In Path Requests
An in Path Request is the most simple of the request types. It converts input parameters into a Url which will be used to consume an API resource, whilst also desalinizing the response into the specified Type if one was supplied. There are four methods currently made available.
>**Take Note:** In Path Requests support all 5 Methods. Post, Get, Put, Patch and Delete.

* The first and most simple of the InPathRequests. It will convert the *paramter* to a string using the *ToString()* method and apply it to the end of the Url. Using the previous settings for out UserApi if the parameter was set to 42 the generated Url would be this.
http://myservice:8080/api/User/42
```csharp
Task<IApiResponse> InPathRequestAsync(Method method, object endpoint = null)
```
* The second request makes the *endpontTemplate* parameter available which supports string formatting on multiple parameters. I cover Endpoint Template more here **TODO**.
>**See More:** [You can read more about string Formatting here](https://www.tutlane.com/tutorial/csharp/csharp-string-format-method).

```csharp
Task<IApiResponse> InPathRequestAsync(Method method, string endpointTemplate, params object[] endpointParameters)
```
* The final two requests below are the same as the above, however are slightly mutated to allow the \<TResponse> Type parameter which tells the **ApiHanlder** to deserialize the JSON contained in the body of the response.
```csharp
Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, object endpoint = null)
```
```csharp
Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, string endpointTemplate, params object[] endpointParameters)
```
### In Body Requests
An in Body Request gives you all the basics of an In Path Request whilst allowing you to serialize the specified Type into JSON which is sent in the body of the request. There are four methods currently made available.
>**Take Note:** In Body Requests only support 3 out of the 5 Methods. Post, Put and Patch. Get and Delete will throw an **ArgumentException**


```csharp
Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, object endpoint = null)
```
```csharp
Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
```
```csharp
Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, object endpoint = null)
```
```csharp
Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
```
### In Path Requests with Query
The most advanced out of the three types of requests. Allowing custom queries from the supplied, whilst also offering built-in functionality to specify what properties are to be used for the query.
>**Take Note:** In Path Requests with Queries supports all 5 Methods. Post, Get, Put, Patch and Delete.

The main difference between the With Query Request and the In Path Request is the addition of the \<TQueryContent> Type parameter and the query expression. The query expression will provide a new object based on the selected properties, the new object will be used for query generation. If there is no query expression provided all of the properties available in content will be used in the query.
```csharp
Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, object endpoint = null)
```
```csharp
Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, string endpointTemplate, params object[] endpointParameters)
```
## CrudApiHandler

Introducing zero code CRUD, included in Serene API is a class called **CrudApiHandler<TResource, TIdentifier>** and when inherited, offers all the basics of CRUD with zero effort required.

Here is an example of implementing the CrudApiHandler using our favorite UserDto.
```csharp
public IUserApi : ICrudApiHandler<UserDto, long>
{
}

public UserApiHandler : CrudApiHandler<UserDto, long>, IUserApi
{
	public UserApiHandler(IApiOptions options) : base(options)
	{
	}
}
```
And that's it, seriously that is all that is needed to implement a basic CRUD API consumer. Here is what it offers.
```csharp
	IApiResponse<UserDto> response = await userApi.GetAsync(171);
```
```csharp
	IApiResponse<List<UserDto>> response = await userApi.GetAllAsync();
```
```csharp
	IApiResponse<UserDto> response = await userApi.CreateAsync(new UserDto());
```
```csharp
	IApiResponse<UserDto> response = await userApi.DeleteAsync(171);
```
```csharp
	IApiResponse<UserDto> response = await userApi.ReplaceAsync(new UserDto());
```
```csharp
	IApiResponse<UserDto> response = await userApi.UpdateAsync(new UserDto());
```
