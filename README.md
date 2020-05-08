# What is Serene API?

Serene API is a small library intended to provide a straightforward way of consuming **RESTful** Apis requiring as little code & setup as possible.

## Getting Started

First off, you'll need to implement an ApiHandler and IApi interface. In this example we'll be consuming the **User** resource. The naming conventions is as follows
* Api Definition: I**Resource**Api
* Api Implementation: **Resource**ApiHandler 

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
		return InPathRequestAsync<UserDto>(ApiMethod.GET, userId);
	}
	
	public Task<IApiResonse<UserDto>> CreateAsync(UserDto user)
	{
		return InBodyRequestAsync<UserDto, UserDto>(ApiMethod.POST, user)
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
The above code will perform two basic actions. It will Get a User using their ID and Create a new user.
Here is the URL GetUserAsync(171) will generate.
>GET : http://myservice:8080/api/User/171

Here is the URL CreateUserAsync(newUser) will generate, the newUser will be serialized to JSON and sent in the body of the request.

>POST : http://myservice:8080/api/User

Take note of how **api/** is being appended to the URL. We'll go over this in a later

Below is what our **UserDto.cs/** file looks like.
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

## The IApiResponse

IApiResponse is core to Serene and is always returned from ApiRequests and it comes in two flavors
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
	UserDto user = new UserDto();

	IApiResponse<UserDto> userResponse = await userApi.GetAsync(171);
	
	IApiResponse<List<UserDto>> usersResponse = await userApi.GetAllAsync();

	IApiResponse<UserDto> createdUserResponse = await userApi.CreateAsync(user);

	IApiResponse<UserDto> deleteUserResponse = await userApi.DeleteAsync(171);

	IApiResponse<UserDto> replacedUserResponse = await userApi.ReplaceAsync(user);

	IApiResponse<UserDto> updatedUserResponse = await userApi.UpdateAsync(user);
```