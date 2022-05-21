# Newtonsoft.Json
Newtonsoft is one of the most well known JSON serialization libraries on Nuget, as such support for it has been added to SereneApi. Once Newtonsoft has been added it will be used to serialize a request and deserialize a response.

## Usage

Firstly you'll need to install the latest version of the nuget package ```SereneApi.Serializers.Newtonsoft.Json```

After the package has been installed Newtonsoft can be added by calling the following method.

```csharp
.RegisterApi<IFooApi, FooApiHandler<(c =>
{
	c.UseNewtonsoftSerializer();
});
```

It is possible to configure Newtonsoft either by explicity provided ```JsonSerializerSettings``` or by using the settings builder.

```csharp
.RegisterApi<IFooApi, FooApiHandler<(c =>
{
	c.UseNewtonsoftSerializer(new JsonSerializerSettings());

	-- OR --

	c.UseNewtonsoftSerializer(s => 
	{
	});
});
```
