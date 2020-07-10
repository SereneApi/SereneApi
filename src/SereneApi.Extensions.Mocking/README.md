SereneApi.Extensions.Mocking
SereneApi.Extensions.Mocking
# Overview
The Mocking package enables mocking with SereneApi, allowing easier development and testing without ever hitting a real API.

This is achieved by adding mock responses to the specified API. You're able to return - Status's, Messages and Content. You can also filter responses for certain requests, this allows specific responses to be returned depending on the request received. You can filter by, Method, URL and Content. It is also possible to add artificial latency to responses.

## Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Extensions.Mocking**

<br/>

Once the package is installed only one new method will be made available, *WithMockResponse*. This method extends *RegisterApi* and *ExtendApi*.
> **NOTE:** Calling *WithMockResponse* more than once will result in the preceding mock responses to be overwritten.

<br/>

**Add Mock Responses During Registration**
```csharp
RegisterApi<IFooApi, IFooApiHandler>().WithMockResponse();
```
**Add Mock Responses Post Registration**
This is useful for adding the mock responses to a test projects as it can be called after the API has been registered.
```csharp
ExtendApi<IFooApi>().WithMockResponse();
```
## Building a Mock Response
Inside the *WithMockResponse* method are two parameters. The mock response factory and a boolean value specifying if outgoing requests are enabled. 
>**NOTE:** If outgoing requests are not enabled, an *ArgumentException* will be thrown if no associated mock response can be found. If it is enabled and no mock response can be found it will perform an actual request against the API.

The mock response factory is what creates the mock responses. Multiple mock responses can be added with varying degrees of response data. Mock response are found using a weight system, the response with the highest weight is uses or if there are multiple with matching weights the response supplied first will be used.
Weights are created using the filter, if no filter is supplied the weight will be 0. If the filter matches it is incremented by 1. If the filter does not match the weight is set to -1 and is skipped.

A mock response can return a Status and/or in body Content. This can be used to return resource from an API or return a status of either successful or unsuccessful. A message can be passed alongside a response.

A mock response's filters can have multiple values provided for each filter - Method, URL and Content. Only one value in each filter is required to match for the weight to be incremented. If none match the value is skipped.
>**NOTE:** Filters are applied in the following order: Method, URL, Content.

Using the *ResponseIsDelayed* method allows the mock response to be delayed increasing the latency. This is intended to allow for more complex development and testing options.
```csharp
.WithMockResponse(r =>
{
	r.AddMockResponse(new Bar())
		// Responds to ALL GET requests.
		.ResponseToRequestsWith(Method.GET);
	r.AddMockResponse(Status.Ok, "Your Custom Message")
		.ResponseIsDelayed(20, 2)
		// Response to POST requests to http://localhost/api/resource/ that contain the specified Bar object.
		.RespondsToRequestsWith(Method.POST)
		.RespondsToRequestsWith("http://localhost/api/resource/")
		.ResponseToRequestWith(new Bar());
}, bool allowOutGoingRequests);
```
Overview
The Mocking package enables mocking with SereneApi, allowing easier development and testing without ever hitting a real API.

This is achieved by adding mock responses to the specified API. You’re able to return - Status’s, Messages and Content. You can also filter responses for certain requests, this allows specific responses to be returned depending on the request received. You can filter by, Method, URL and Content. It is also possible to add artificial latency to responses.

Getting Started
Add the latest version of the NuGet package to your project.

PM> Install-Package SereneApi.Extensions.Mocking


Once the package is installed only one new method will be made available, WithMockResponse. This method extends RegisterApi and ExtendApi.

NOTE: Calling WithMockResponse more than once will result in the preceding mock responses to be overwritten.


Add Mock Responses During Registration

RegisterApi<IFooApi, IFooApiHandler>().WithMockResponse();
Add Mock Responses Post Registration
This is useful for adding the mock responses to a test projects as it can be called after the API has been registered.

ExtendApi<IFooApi>().WithMockResponse();
Building a Mock Response
Inside the WithMockResponse method are two parameters. The mock response factory and a boolean value specifying if outgoing requests are enabled.

NOTE: If outgoing requests are not enabled, an ArgumentException will be thrown if no associated mock response can be found. If it is enabled and no mock response can be found it will perform an actual request against the API.

The mock response factory is what creates the mock responses. Multiple mock responses can be added with varying degrees of response data. Mock response are found using a weight system, the response with the highest weight is uses or if there are multiple with matching weights the response supplied first will be used.
Weights are created using the filter, if no filter is supplied the weight will be 0. If the filter matches it is incremented by 1. If the filter does not match the weight is set to -1 and is skipped.

A mock response can return a Status and/or in body Content. This can be used to return resource from an API or return a status of either successful or unsuccessful. A message can be passed alongside a response.

A mock response’s filters can have multiple values provided for each filter - Method, URL and Content. Only one value in each filter is required to match for the weight to be incremented. If none match the value is skipped.

NOTE: Filters are applied in the following order: Method, URL, Content.

Using the ResponseIsDelayed method allows the mock response to be delayed increasing the latency. This is intended to allow for more complex development and testing options.

.WithMockResponse(r =>
{
	r.AddMockResponse(new Bar())
		// Responds to ALL GET requests.
		.ResponseToRequestsWith(Method.GET);
	r.AddMockResponse(Status.Ok, "Your Custom Message")
		.ResponseIsDelayed(20, 2)
		// Response to POST requests to http://localhost/api/resource/ that contain the specified Bar object.
		.RespondsToRequestsWith(Method.POST)
		.RespondsToRequestsWith("http://localhost/api/resource/")
		.ResponseToRequestWith(new Bar());
}, bool allowOutGoingRequests);
Markdown 3356 bytes 488 words 52 lines Ln 12, Col 126HTML 2748 characters 476 words 34 paragraphs