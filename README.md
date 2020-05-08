# What is Serene API?

Serene API is a small library intended to provide a straightforward way of consuming **RESTful** APIs requiring as little code & setup as possible whilst providing a powerful set of tools.

## The Basics

First off, you'll need to implement an ApiHandler and IApi interface. In this example we'll be consuming the **User** resource. The naming conventions is as follows
* Api Definition: I**Resource**Api
* Api Implementation: **Resource**ApiHandler 
>**Best Practices Tip:** Your ApiHandler Implementations should be located in a Handlers folder under your project.

Defining the IUserApi
```csharp
public interface IUserApi
{
	Task<IApiResponse<UserDto>> GetAsync(long userId);
	
	Task<IApiResonse<UserDto>> CreateAsync(UserDto user);
}
```
Implementing the UserApiHandler
```csharp
public class UserApiHandler : ApiHandler, IUserApi
{
	public UserApiHandler(ApiHandlerOptions<UserApiHandler> options) : base(options)
	{
	}
	
	public Task<IApiResposne<UserDto>> GetAsync(long userId)
	{
		return InPathRequestAsync<UserDto>(ApiMethod.Get, userId);
	}
	
	public Task<IApiResonse<UserDto>> CreateAsync(UserDto user)
	{
		return InBodyRequestAsync<UserDto, UserDto>(ApiMethod.Post, user)
	}
}
```
Once you've created the API Definition and Implementation you can initiate it in your **Startup.cs** file.
```csharp
services.AddApiHandler<IUserApi, UserApiHandler>((provider, builder) => 
{
	builder.AddConfiguration(Configuration.GetApiConfig("UserApi"));
	builder.AddLoggerFactorty(provider.GetRequiredService<ILoggerFactory>());
});
```
In the example above we're getting our API Configuration from **appsettings.json** and adding an **ILoggerFactory** so logging is enabled.

Here is what the **appsettings.json** file looks like
```json
"ApiConfig": {
  "UserApi": {
    "Source": "http://myservice:8080",
    "Resource": "User"
  }
}
```
>**Take Note:** You can also provide ResourcePrecursor:*string* and Timeout:*TimeSpan* under your API configuration. But they're entirely optional.

The above code will perform two basic actions. It will Get a User using their ID and Create a new user. Below is an example of the Url that this code will act upon.

>**Take Note:** Between the Source and Resource **api/** is being appended to the URL. We'll go over this later when we cover the **ResourcePrecursor**.

Here is the URL that GetUserAsync(171) will generate.
>GET : http://myservice:8080/api/User/171

Here is the URL that CreateUserAsync(newUser) will generate, the newUser object will be serialized to JSON and sent in the body of the request.

>POST : http://myservice:8080/api/User

For clarity below is what the **UserDto.cs/** file looks like.
>**Best Practices Tip:** Your DTOs should be called **Resource**Dto
```csharp
public class UserDto
{
	public long ID { get; set; }
	
	public string FirstName{ get; set; }

	public string LastName{ get; set; }
	
	public string UserName{ get; set; }

	public string Email { get; set; }

	public DateTime BirthDate { get; set; }
}
```
## The ApiHandler

The **ApiHandler** is a core part of **SereneApi** and handles every single request. When inherited it offers a plethora of simple to use and yet powerful methods that should cover most if not all use cases and is completely extensible.
>**Take Note:** No examples will be provided in this chapter. Review the Examples section to see it in action.

### ApiMethods
The ApiMethod Enum tells the ApiHandler what **RESTful** method it will be performing in the requet. Below are the methods made available in **SereneApi**
```csharp
public enum ApiMethod
{
	Post,
	Get,
	Put,
	Patch,
	Delete
}
```
### In Path Requests
An in Path Request is the most simple of the request types. It converts input parameters into a Url which are used to API consumption whilst also desalinizing the response into the specified Type if one was supplied. There are four methods currently made available in **SereneApi**
>**Take Note:** In Path Requests support all 5 Methods. Post, Get, Put, Patch and Delete.

The first and most simple of the InPathRequests. It will convert the **paramter** to a string using the *ToString()* method and apply it to the end of the Url. Using our previous settings for out UserApi if the paramter was set to 42 the generated Url would be this.
http://myservice:8080/api/User/42
```csharp
Task<IApiResponse> InPathRequestAsync(ApiMethod method, object parameter = null)
```
The second request makes the **actionTemplate** parameter available which supports string formatting and multiple parameters. I'll cover Action Templates more later on.
>**See More:** [You can read more about string Formatting here](https://www.tutlane.com/tutorial/csharp/csharp-string-format-method).

```csharp
Task<IApiResponse> InPathRequestAsync(ApiMethod method, string actionTemplate, params object[] parameters)
```


The final two requests below are the same as the above, however they are slightly mutated to allow the \<TResponse> Type parameter which allows the **ApiHanlder** to deserialize the JSON contained in the Body of the Response; which is then returned in the IApiResponse.
```csharp
Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, object parameter = null)
```
```csharp
Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, string actionTemplate, params object[] parameters)
```

### In Body Requests
An in Body Request gives you all the basics of an In Path Request while allowing you to serialize the specified Type into JSON which sent in the body of the request. There are four methods currently made available in **SereneApi**
>**Take Note:** In Body Requests only support 3 out of the 5 Methods. Post, Put and Patch. Get and Delete will throw an **ArgumentException**

The first of the In Body Requests will serialize the **inBodyContent** parameter into JSON which will be sent in the body of the request.
```csharp
Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, TContent inBodyContent)
```
The second of the in Body Requests makes the **action** parameter available which allows the **ApiHandler** to route it to the correct **RESTful** API method.
>**Take Note:** An Action is does not support templating.
```csharp
Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, object action, TContent inBodyContent)
```
As before, the below two methods are the same as the previous however add the \<TResponse> Type parameter which allows the **ApiHanlder** to deserialize the JSON contained in the Body of the Response; which is then returned in the IApiResponse.
```csharp
Task<IApiResponse<TResponse>> InBodyRequestAsync<TResponse, TContent>(ApiMethod method, TContent inBodyContent)
```
```csharp
Task<IApiResponse<TResponse>> InBodyRequestAsync<TResponse, TContent>(ApiMethod method, object action, TContent inBodyContent)
```
### In Path Requests with Query
The most advanced out of the three types of requests. It allows you to convert the supplied type into a query whilst also offering you in-built tools to specify what properties of the type you'd like to make part of the Query.
>**Take Note:** In Path Requests with Queries supports all 5 Methods. Post, Get, Put, Patch and 

Delete.
The main difference between the With Query request and the stand In Path Request is the addition of \<TContent> Type paramter and the query expression. The query expression will provide a new object based on the selected properties from the content. If there is no query expression provided all of the properties available in content will be used to create the query.
```csharp
Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TContent>(ApiMethod method, object action, TContent content, Expression<Func<TContent, object>> query = null)
```
```csharp
Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TContent>(ApiMethod method, string actionTemplate, TContent content, Expression<Func<TContent, object>> query = null, params object[] parameters)
```

## The IApiResponse

IApiResponse is always returned from ApiRequests and it comes in two flavors
```csharp
// To be used when you're not going to deserialize an object from the response.
IApiResponse

// To be used when you're going to deserialize an object from the response.
IApiResponse<TResponse>
```
You don't need to do anything with this as ApiHandler will do all of the work in the background. So how do you use IApiResponse?
```csharp
	IApiResponse userResposne<UserDto> = await userApi.GetAsync(171);
	
	// Check if the request has failed.
	if(!userResponse.WasSuccessful)
	{
		// Check if the resposne has an exception		
		if(userResponse.HasException)
		{
			// Get the Exception
			Exception exception = userResponse.Exception;
		}
		
		string message = userResponse.Message;
	}

	UserDto user = userResponse.Result;
``` 

IApiResonse.WasSuccessful;
>Returns a boolean value specifying if the request was successful.

IApiResonse.HasException;
>Returns a boolean value specifying if the request has an exception.

IApiResonse.Exception;
>Contains the Exception returned from the ApiHandler.

IApiResonse.Message;
>Contains a string value that is received either from the ApiHandler or the service you're consuming.

IApiResonse\<TResponse>.Result;
>Contains the deserialized result of the response. This is the only property that isn't shared between the two IApiResponses and is only available on the Generic IApiResonse


## The CrudApiHandler

Introducing zero code CRUD, included in Serene API is a class called **CrudApiHandler<TResource, TIdentifier>** and when inherited, offers all the basics of CRUD with zero effort required.

Here is an example of implementing the CrudApiHandler using our favorite UserDto.
```csharp
public IUserApi : ICrudApiHandler<UserDto, long>
{
}

public UserApiHandler : CrudApiHandler<UserDto, long>, IUserApi
{
	public UserApiHandler(ApiHandlerOptions<UserApiHandler> options) : base(options)
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

## Intermediate Usage

## Examples

# Planed Features
* Extend In Body Requests to allow Action Templates and Parameters.
* Unit Tests, at the present time there is no automated testing being done.
* Make it available on NUGET!