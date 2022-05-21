# Windows Impersonation
Enables windows to authenticate requests by impersonating the current windows identity when sending the request.
> **NOTE**: Only the request execution is impersonated, all other code is executed normally.

## Usage

To start using windows impersonation the first step is to install the latest nuget package for ```SereneApi.Extensions.WindowsImpersonation```.

After the pack is installed it is simple as calling the ```UseWindowsImpersonation``` method during Handler Configuration. Below are some examples.

> **NOTE**: This package is only supported in a windows environment. Execution outside of a windows environment will throw a **NotSupportedException**

> **NOTE**: The **IHttpContextAccessor** interface must be present, if it is not an attempt will be made to retrieved it from the **IServiceProvider** if running in AspNet.

### Amend Configuration
Enables windows impersonation on all Handlers using the specified Configuration Provider before Registration takes place.
```csharp
.AmendConfigurationProvider<RestHandlerConfigurationProvider>(c => 
{
	c.UseWindowsImpersonation();
});
```
### Registration
Enables windows impersonation for the specified ApiHandler during Registration.
```csharp
.RegisterApi<IFooApi, FooApiHandler>(c => 
{
	c.UseWindowsImpersonation();
});
```
### Extensions
Enables windows impersonation for the specified ApiHandler after Registration takes place.
```csharp
.ExtendApi<FooApiHandler>(c => 
{
	c.UseWindowsImpersonation();
});
```
