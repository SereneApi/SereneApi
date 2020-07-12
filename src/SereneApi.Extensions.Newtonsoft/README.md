# Getting Started

Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Extensions.Newtonsoft**

<br/>

To add Newtonsoft, put the following code in your Startup.cs file. This will override the default *ISerializer* with the *NewtonsoftSerializer*.
<br/>
**Add Newtonsoft.Json as the default serializer**
```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.ConfigureSereneApi().AddNewtonsoft();
```
**Add Newtonsoft.Json to a specific API**
```csharp
RegisterApi<IFooApi, FooApiHandler>(builder =>
{
	builder.UseNewtonsoftSerializer();
```
Serialization settings can be provided as a method parameter by using either a *JsonSerializationSettings* object or a lambda expression.
## Special Thanks
* To the [Newtonsoft.Json](https://www.newtonsoft.com/json) team.