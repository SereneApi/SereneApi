# Setting up Dependency Injection
There are no changers to the ApiHandler or its implementation, meaning existing code can be moved across with little change.

There are only two major changes regarding setting up a handler.
1. Your ApiHandler constructor needs to use the generic implementation of IApiHandlerOptions.
```csharp
public UserApiHandler(IApiHandlerOptions<UserApiHandler> options) : base(options)
{
}
```
2. The ApiHandlerFactory is no longer required as dependency injection supercedes its use. Instead the handlers are configured in the ConfigureServices method under Startup.cs. To add an ApiHandler simply call services.AddApiHandler<>() it is similar to the method on the ApiHandlerFactory except it requires a TDefinition and adds some extra options.
```csharp
services.AddApiHandler<IStudentApi, StudentApiHandler>(o =>
{
	o.UseConfiguration(Configuration.GetApiConfig("Student"));
});
```
