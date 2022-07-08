# MSAL WebAssebly Authentcation
This package allows an ApiHandler to have its requests authenticated against Microsoft Azure, whilst this section will cover how to enable MSAL authentication it will not go into details on how to configure Authentication in Azure. The following prerequisites are to be meet if configuration is to be done.

1.	Have the ClientId as provided by Azure.
2.	Have the User Scope(s) as provided by Azure.

## Usage

    To start using Msal authentication the first step is to install the latest nuget package for ```SereneApi.Authentication.WebAssenly.Msal```.

### Configure Msal Authentication

There are two ways to configure Msal Authentication, either with the fluent api or by using the IApiAuthorization interface.

### Fluent API
    ```csharp
    .RegisterApi<IFooApi, FooApiHandler>(c =>
    {
        c.UseMsalAuthentication("{ClientId}", o => 
        {
            o.ReturlUrl = "{ReturnUrl}";
            o.RegisterScope("{UserScope1}";
            o.RegisterScope("{UserScope2}"'
        });
    });
    ```
**ReturlUrl ** 
    Specifies the Return Url of the application post authentication.
**RegiserUserScope**
    Registering the user scope can called as many times as required and uses the following template ```api://{ClientId}/{scope}```

### IApiAuthorization
To use this approach the ```IApiAuthorization``` interface must be present in the ApiHandlers DependencyCollection.
    ```csharp
    .AmendConfigurationProvider<MyConfigurationProvider>(c => 
    {
        c.Dependencies.AddSingleton<IApiAuthorization>(() => 
            new MyApiAuthorization()
            {
                ClientId = "{ClientId}",
                Scopes = new List<string>()
                {
                    "{UserScope1}",
                    "{UserScope2}"
                }
            });
    });

    .RegisterApi<IFooApi, FooApiHandler>(c =>
    {
        bool enableClientAuthentication;

        c.UseMsalAuthentication(enableClientAuthentication)
    });
    ```