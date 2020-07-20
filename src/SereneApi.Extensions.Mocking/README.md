# Overview
The Mocking package enables mocking, allowing easier development and testing without ever interacting with a real API.

This is achieved by adding mock responses to the specified API. It is possible to return - Status's, Messages and Content. You can also filter requests allowing specific responses to be returned depending on the request received. Filtering can be done for Method's, URL's and Content. It is also possible to add artificial latency.

## Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Extensions.Mocking**

Once the package is installed only one  new method will become available, *WithMockResponse*. This method extends *RegisterApi* and *ExtendApi*.
> **NOTE:** Calling *WithMockResponse* more than once will result in the preceding responses to be overwritten.

<br/>

### Add Mock Responses During Registration
```csharp
.RegisterApi<IFooApi, IFooApiHandler>().WithMockResponse();
```
### Add Mock Responses Post Registration
This is useful for adding the mock responses to test projects as it can be called after the API has been registered.
```csharp
.ExtendApi<IFooApi>().WithMockResponse();
```
>**BEST PRACTICE:** Using the *ExtendApi* method is prefered as it can be easily added or removed.
## Building a Mock Response
Inside the *WithMockResponse* method are two parameters. The mock response factory and a boolean value specifying if outgoing requests are enabled by default this is set to false. 
>**NOTE:** If outgoing requests are not enabled, an *ArgumentException* will be thrown if no associated mock response can be found. If enabled and no mock response can be found it will perform a request against the API.

The mock response factory is what creates the mock responses. Multiple mock responses can be added with varying degrees of response data. Mock response are found using a weight system, the response with the highest weight is used or if there are multiple matching weights the response supplied first will be used.
Weights are created using the specified filters, if no filter is supplied the weight will be 0. If the filter matches it is incremented by 1. If the filter does not match the weight is set to -1 and is skipped.

A mock response can return a Status and/or in body Content. This can be used to return resource from an API or return a status of either successful or unsuccessful. A message can be passed alongside a response.

A mock response's filters can have multiple values provided - Method, URL and Content. Only one value in each filter is required to match for the weight to be incremented. If none match the value is skipped.
>**NOTE:** Filters are applied in the following order: Method, URL, Content.

Using the *ResponseIsDelayed* method allows the mock response to be delayed by implementing artificial latency. This is intended to allow for more complex development and testing options.
```csharp
.WithMockResponse(r =>
{
	r.AddMockResponse(new Bar())
		// Responds to ALL GET requests.
		.ResponseToRequestsWith(Method.GET);
	r.AddMockResponse(Status.Ok, "A new bar has been added")
		.ResponseIsDelayed(20, 2)
		// Response to POST requests to http://localhost/api/bar/ that contain the specified Bar object.
		.RespondsToRequestsWith(Method.POST)
		.RespondsToRequestsWith("http://localhost/api/bar/")
		.ResponseToRequestWith(new Bar());
}, bool allowOutGoingRequests);
```