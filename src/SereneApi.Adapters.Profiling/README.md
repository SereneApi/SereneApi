# Overview
This packages enables event-based profiling to be used on your APIs enabling easier and more complete unit testing. Once a profiling session has been started, all requests will be tracked and usage statistics collected. Once a session has been completed an in memory object will be provided containing the collated information during the session.
>**NOTE:** Due to the profilers nature only one session may be run at a time. 

## Getting Started
Add the latest version of the NuGet package to your project.
>PM> Install-Package **SereneApi.Adapters.Profiling**

Before you're able to use the *ProfilingAdapter* class you must first call the below code.
```csharp
.ConfigureApiAdapters(configure =>
{
	configre.InitiateProfilingAdapter();
	// OR
	ProfilingAdapter.Initiate(configure);
});
```
## Usage
### Running the Profiler
Once the *ProfilingAdapter* has been initiated you are able to call the *StartSession* method. There are two ways currently to profile your code. 
* **Run Profiler by Starting and Stopping**
```csharp
ProfilingAdapter.StartSession();

// Your code that you are running the profiler against.

ISession session = ProfilingAdapter.EndSession();
```
* **Run Profiler for Section**
Has Asynchronous and Synchronous.
```csharp
ISession session = ProfilingAdapter.Profile(() => 
{
	// Your synchronous code that you are running the profiler against.
});
```
```csharp
ISession session = await ProfilingAdapter.ProfileAsync(async () => 
{
	// Your asynchronous code that you are running the profiler against.
});
```
### Checking the Usage Information
Once a session has been completed an *ISession* will be returned, the session contains all of the statistical data collected during the duration of the session.
