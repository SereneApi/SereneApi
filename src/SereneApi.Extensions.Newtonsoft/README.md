# Getting Started

Add the latest version of the NuGet package to your project.
>PM> Install-Package SereneApi.Extensions.Newtonsoft

To add Newtonsoft call the following in your Startup.cs file. This will override the default *ISerializer* with the NewtonsoftSerializer

**Add Newtonsoft.Json as the default serializer.**
```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.ConfigureSereneApi().AddNewtonsoft();
```
**Add Newtonsoft.Json to a specific API.**
```csharp
services.RegisterApiHandler<IFooApi, FooApiHandler>(builder =>
{
	builder.UseNewtonsoftSerializer();
```
## Special Thanks
* To the [Newtonsoft.Json](https://www.newtonsoft.com/json) team.