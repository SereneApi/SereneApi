# Abstraction
The SereneApi.Abstraction library contains the necessary interfaces required for making an API consumer without all of the extra *stuff*. This should only be needed if you're making a project only for your DTOs and API interfaces etc.


## IApiResponse

IApiResponse will always be returned from any requests and it comes in two flavors
```csharp
// To be used when you're not going to deserialize an object from the response.
IApiResponse

// To be used when you're going to deserialize an object from the response.
IApiResponse<TResponse>
```
You don't need to do anything with this as the ApiHandler will do all of the work in the background. So how do does IApiResponse work?
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
		
		// Get the Message
		string message = userResponse.Message;
	}

	// Get the Result
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
>Contains the deserialized result of the response. This is the only property that isn't shared between the two response types and is only made available on the Generic IApiResonse

