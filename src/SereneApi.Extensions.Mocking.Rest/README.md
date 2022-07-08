# Rest Mocking
This section covers how to implement Rest API mocking, SereneApi includes an extensive mocking system that uses the entire stack. All the way down to the HttpMessageHandler, this ensures you get both a rough estimate of performance and that the entire API stack will function as intended.

The mocking library offers two ways to mock your API endpoints, either by configuring each Mock Response to a specific request or a MockRestApiHandler. This documentation will introduce both concepts.

## Getting Started
To get started firstly you'll need to install the latest version of ```SereneApi.Extensions.Mocking.Rest``` in your project using Nuget.

After this has done you can either enable mocking during Registration or later by Extended an already Registered API. This can be done by using the ```ApiFactory``` or ```IServiceCollection``` in the example below we'll be using the later.
```csharp
// Specify if the Mocking Middle-ware will allow outgoing requests.
bool enableOutgoing = true/false;

services.RegisterApi<IFooApi, FooApiHandler>(c =>
{
	c.EnableMocking(c => 
	{
	}, enableOutgoing);
});

-- OR --

services.ExtendApi<FooApiHandler>(c =>
{
	c.EnableMocking(c => 
	{
	}, enableOutgoing);
});
```
In the above examples there's a variables called ```enableOutgoing``` this variable specifies if the Mocking Middle-ware will allow an unmatched request to be handled normally. By default this is set to false and if no Mock Response is found a 404 (Not Found) will returned for the request. The idea behind adding this option is to enable a mixed development cycles of both Mocked and Real API calls.

The next thing of note is the lambda expression, all of the Mocking configuration is completed inside within the lambda. By default two methods are made available ```RegisterMockResponse``` and ```RegisterMockingHandler``` both of these methods will be further explained in the next section.

For now we'll go into details on how a Mock Response is matched to a Request. Currently there are three parts of a request taken into account when matching.
1.	The method.
2.	The endpoint.
3.	The Body.

>**SEE:** At present, all other elements of a request are not matched.

Each request is weighted against a response and the highest weighted response will be returned. Each response is given a weight of -1 to 3 where 3 is a perfect match and -1 is not a match. When a weight of -1 is given to a Mock Response it is skipped and no further matching is done. Let me explain how weighing is completed.
*	If the Mock method matches the request the weight is incremented by 1.
*	If the Mock method does not match the request the weight is set to -1 and no further matching will be performed.
*	If the Mock method is not present the weight is not incremented and matching will continue.

If a Mock response is given a score of 3 *(The maximum)* it will be instantly returned. Each Mock Response is matched to a Request in the following order. Method **->** Endpoint **->** Content. It is also possible to assigned multiple Methods and Endpoints to a Mock Response. It is also possible to check one or all of the variables. As an example you could have a Mock Response that only matched to a Get Request or some specified Body content.

Lastly, it is also possible to assign a delay to a given Mock Response. A delay is set using a ```TimeSpan```. When the Mock Response is matched the thread will be help for the delay period before returning the response. It is also possible to specify how many times a delay will be executed for the lifetime of an ApiHandler. As an example, let's say you have an ApiHandler with a timeout of 5 seconds and you have a Mock Response with a delay of 10 seconds. Obviously the request will timeout every time you try and make the request. However, if you set the repeats to 2, on the third request no delay will be applied.



## RegisterMockResponse
The ```RegisterMockResponse``` method uses a FluentApi approach for configuration, whilst the order of methods is necessary the only method that is required is ```RespondsWith```. Below is an example of usage.
```csharp
.EnableMocking(c => 
{
	c.RegisterMockResponse()
		.ForMethod(Method.Get)
		.ForEndpoints("some/endpoint")
		.ForContent(new Foo())
		.RespondsWith(new Bar())
		.IsDelayed(2);
});
```
As shown in the above example there are 5 methods used to fully configure a Mock Response. The first three methods are used to describe what request the response should be reply to and the last two methods explain how and what to reply with.
### The FluentApi
*	**ForMethod** - *Optional*
	This method specifies what  Method(s) will need to match the given request for this Mock Response to respond.
*	**ForEndpoints** - *Optional*
		This method specifies what  Endpoint(s) will need to match the given request for this Mock Response to respond.
		
*	**ForContent** - *Optional*
	This method specifies what Body Content will need to match the given request for this Mock Response to respond.

*	**ResponseWith** - *Required*
	This method specifies how a Mock Response will respond to a matching request. You can either specify a Status and/or an object to be returned.
*	**IsDelayed** - *Optional*
	This method specifies how long a Mock Response will be delayed by and how many times the delay will be applied during the lifetime of an ApiHandler.
## RegisterMockingHandler
The ```RegisterMockResponse``` method enables more in-depth mocking to be accomplished, this is achieved by the introduction of the ```MockRestApiHandler```. The Mocking Handler closely represent how an ApiController is implemented and supports Dependency Injection.

> **NOTE**: The mocking handler has a lifetime of Scoped. Meaning that when the ApiHandler it is mocking is disposed so will the Mock Handler. If you data to be retained either store it as a Singleton Dependency or a static parameter.

The way the Mock Handler works follows very closely to the previous method but with one caveat. A Mock Handler may only ever get a response weight of 2. Meaning that if you have a Mock Response registered using the previous method that matches all three variables it will be used instead.

A Mock handler will only be called if the request matches both the Method and the Endpoint, once matching is passed the requests parameters, body and query will be bound to the parameters of the endpoints method and invoked either synchronously or asynchronously. A Mock Handler **can only** return either a ```IMockResult``` or ```Task<IMockResult>``` - If any other type is returned an exception will be thrown. Only public methods can be used and methods without the Mock*Status*Attribute will ignored. The following attributes may be used.
*	**[MockDelete]** - *Method*
	Specifies that the method will respond to Delete request, you can also set the endpoint to be replied to. The endpoint can be bound to the method parameters.
	
*	**[MockGet]** - *Method*
	Specifies that the method will respond to Get request, you can also set the endpoint to be replied to. The endpoint can be bound to the method parameters.

*	**[MockPatch]** - *Method*
	Specifies that the method will respond to Patch request, you can also set the endpoint to be replied to. The endpoint can be bound to the method parameters.

*	**[MockPost]** - *Method*
	Specifies that the method will respond to Post request, you can also set the endpoint to be replied to. The endpoint can be bound to the method parameters.

*	**[MockPut]** - *Method*
	Specifies that the method will respond to Put request, you can also set the endpoint to be replied to. The endpoint can be bound to the method parameters.

> **NOTE**: All Method Attributes set both the Method and the Endpoint to be used for matching.

> **NOTE**: All Method Attributes allow the endpoint to be bound to the method parameters. Binding is done by adding the matching parameter name to the endpoint and surrounding it with curly brackets. EG: {firstName} would be bound to (string firstName). Casing is not ignored.


*	**[IsDelayed]** - *Method*
	Specifies that the request is delayed by the specified amount.

*	**[FromBody]** - *Parameter*
	Specifies that the parameter will be bound to the body of the request.

*	**[FromQuery]** - *Parameter*
	Specifies that the parameter will be bound to the query of the request.


Below is a basic example of a ```MockRestApiHandler```, as you can see our Mock Handler inherits the base class ```MockRestApiHandlerBase```. This is required. The base handler provides some basic methods to make returning Mock Results a little easier. Methods like Ok, NotFound, StatusCode.

```csharp
internal class MockFooRestApiHandler : MockRestApiHandlerBase
{
	private readonly ILogger _logger;

	public MockFooRestApiHandler(ILogger logger)
	{
		_logger = logger;
	}
	
	[IsDelay(500)]
	[MockGet("{id}")]
	public async Task<IMockResult> GetByAgeAsync(int id)
	{
		//Execute Async Code

		return Ok(myFoo);
	}
}
```
Now that we've seen a basic implementation let's register it.
```csharp
.EnableMocking(c => 
{
	c.RegisterMockingHandler<MockFooRestApiHandler>();
});
```
### Dependency Injection
Whilst the Mock Handler does support Dependency Injection it can only retrieve dependencies assigned to the ApiHandler it is mocking. This means if you want a dependency that is contained in AspNet's IDependencyCollection you will need to do some extra configuration.
```csharp
public MockFooRestApiHandler(IServiceProvider provider)
{
	provider.GetRequiredService<IMyService>();
}
```
```csharp
services.ExtendApi<FooApiHandler>((c, p) => 
{
	c.Dependencies.AddScoped<IMyService>(() => p.GetRequiredService<IMyService>(), Binding.Unbound);

	-- or --

	c.Dependencies.AddScoped<IMyService, MyService>();
});

-- --

public MockFooRestApiHandler(IMyService myService)
{
}
```

### Conclusion
To conclude the overview of the Mock handler it is intended to analogous to your stand ApiController. There are some minor differences, this was done to signify a clear difference in responsibility.
