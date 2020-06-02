
# What is SereneApi?

SereneApi is intended to provide a straightforward way of consuming **RESTful** APIs requiring as little code & setup as possible whilst providing a powerful set of extensible tools.

**Why**
I found that creating Web Requests and Deserializing/Serializing tended to become tedious and in some cases break DRY. I also disliked how it usually made my code base look cluttered.
Most other implementation of RESTful libraries usually gave me a similar clutter like vibe which I wanted to avoid.

Serene removes this code clutter allowing for clean single line methods to be used for accessing resources; After using the Repository Pattern for many years I was inspired to create a API Handler that followed a similar pattern, this is what led to SereneApi.

**Current Releases**
* SereneApi | *Environment* - Standard 2.1
>Adds  **SereneApi** to your project.  <br>
[![Nuget](https://img.shields.io/nuget/v/SereneApi.svg?style=flat-square)](https://www.nuget.org/packages/SereneApi/)
* SereneApi.Abstraction | *Environment* - Standard 2.1
>Contains abstracted components.  <br>
[![Nuget](https://img.shields.io/nuget/v/SereneApi.Abstraction.svg?style=flat-square)](https://www.nuget.org/packages/SereneApi.Abstraction/)
* SereneApi.DependencyInjection | *Environment* - Core 3.1
>Extends **SereneApi** adding support for AspNet Dependency Injection.  <br>
[![Nuget](https://img.shields.io/nuget/v/SereneApi.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/SereneApi.DependencyInjection/)
* SereneApi.Extras | *Environment* - Standard 2.1
>Adds Extras and Helpers that are involded with consuming specific 3rd party APIs.  <br>
[![Nuget](https://img.shields.io/nuget/v/SereneApi.Extras.svg?style=flat-square)](https://www.nuget.org/packages/SereneApi.Extras/)
# Planed Features
* Unit Tests...
* More control over logging, are logs being generated for exceptions and messages.
* ~~Extend In Body Requests to allow Action Templates and Parameters.~~
* ~~Improve Options adding Generic Dependencies.~~
* ~~Make it available on NUGET!~~
* ~~Separate DI from the base library. Add in Factories for ApiHandler Creation.~~
#
### Special Thanks

* Nuget Icons made by <a href="https://www.flaticon.com/authors/prosymbols" title="Prosymbols">Prosymbols</a> from <a href="https://www.flaticon.com/" title="Flaticon"> www.flaticon.com</a>