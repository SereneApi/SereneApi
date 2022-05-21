# Blazor WebAssembly Support
Blazor does not support some of the features that SereneApi may use. This package has been added to ensure compatible with the default implementation of SereneApi.

## Usage
Firstly you'll need to install the latest version of the nuget package ```SereneApi.Extensions.Blazor```

To enable Blazor support simply call the following method during configuration.

```csharp
.RegisterApi<IFooApi, FooApiHandler>(c => 
{
	c.EnableBlazorWebAssemblySupport();
});
```

Calling this method will ensure the following is done.
*	Removes any instance of ```ICredentials```
*	Replaces Newtonsoft with the default JsonSerializer.
*	Disables Windows Impersonation.