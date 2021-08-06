

# Common Questions
Below is a list of common questions asked about SereneApi.
*	**[How do I use queries?](#how-do-I-use-queries)**
*	**[How do endpoints work?](#how-do-endpoints-work)**
*	**[How do I add content to the body of a request?](#how-do-i-add-content-to-the-body-of-a-request)**
*	**[How do I receive content from a request?](#how-do-i-receive-content-from-a-request)**
*	**[How do I setup an API?](#how-do-i-setup-an-api)**
*	**[How do I register an API?](#how-do-i-register-an-api)**
*	**[Can I use CRUD?](#can-i-use-crud)**
*	**[Can I use different versions of my APIs?](#can-i-use-different-versions-of-my-apis)**
*	**[Does SereneApi have a best practices guide?](#does-sereneapi-have-a-best-practices-guide)**
*	**[How does authentication work?](#how-does-authentication-work)**

# How do I use queries?
Queries can be added to a request using the WithQuery() method. There are several ways a query can be added to a request which is demonstrated below.

However, before we look at the examples, first let's outline an object to be used.
``` csharp
public class StudentDto
{
	public long Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime BirthDate { get; set; }
}
```

### 1. Create query from an object
``` csharp
public Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto query)
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithQuery(query)
		.RespondsWithType<List<StudentDto>>()
		.ExecuteAsync();
}
```
Generated query string.
```
?Id=1&FirstName=John&LastName=Smith&BirthDate=1980-01-05
```

### 2. Create query using selected object properties
``` csharp
public Task<IApiResponse<List<StudentDto>>> SearchAsync(StudentDto query)
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithQuery(query, o => new { o.FirstName, o.LastName, o.BirthDate })
		.RespondsWithType<List<StudentDto>>()
		.ExecuteAsync();
}
```
Generated query string.
```
?FirstName=John&LastName=Smith&BirthDate=1980-01-05
```
### 3. Create query using an anonymous object
``` csharp
public Task<IApiResponse<StudentDto>> SearchAsync(long id, string fullName)
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithQuery(new { Identity = id, Name = fullName })
		.RespondsWithType<StudentDto>()
		.ExecuteAsync();
}
```
Generated query string.
```
?Identity=1&Name=John Smith
```

# How do endpoints work?
Endpoints work in one of three ways. Each way determines how the endpoint will be generated for the request. But in all cases a string and or parameters are provided.

 Some things to note regarding endpoints and there usage.
1.	Endpoints are placed after the source, version and resource; and before the query.
2.	string formatting is used to generate the endpoint, see [string.Formatting](https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=net-5.0) for more details on its use.
3.	If an endpoint string is not provided, only one Parameter may be used.
4.	If an endpoint is provided without formatting and one parameter is given it will be appended to the end.

### 1. Basic string endpoint
``` csharp
public Task<IApiResponse<List<ReportDto>>> GetReportsAsync()
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithEndpoint("Reports")
		.RepospondsWithType<List<ReportDto>>()
		.ExecuteAsync();
}
```
### 2. Parameter endpoint
``` csharp
public Task<IApiResponse<FooDto>> GetAsync(long id)
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithParameter(id)
		.RespondsWithType<FooDto>()
		.ExecuteAsync();
}
```
### 3. Formatted string endpoint
``` csharp
public Task<IApiResponse<DateTime>> GetBirthDateAsync(long id)
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.WithEndpoint("{0}/BirthDate")
		.WithParameter(id)
		.RespondsWithType<DateTime>()
		.ExecuteAsync();
}
```

# How do I add content to the body of a request?
Adding content to the body of a request can be done using the AddInBodyContent\<*TContent*>() method. After this method is called it will serialize the provided  object into a json formatted string using the *ISerializer*.
``` csharp
public Task<IApiResponse> CreateAsync(StudentDto student)
{
	return MakeRequest
		.UsingMethod(Method.Post)
		.AddInBodyContent(student)
		.ExecuteAsync();
}
```

# How do I receive content from a request?
You can received content from the string using the RespondsWithType\<*TContent*>() method. The value received will be deserialized into the specified value using the *ISerializer*.
``` csharp
public Task<IApiResponse<List<StudentDto>>> GetAsync()
{
	return MakeRequest
		.UsingMethod(Method.Get)
		.RespondsWithType<List<StudentDto>>()
		.ExecuteAsync();
}
```

# How do I setup an API?

# How do I register an API?

# Can I use CRUD?
Yes you can, SereneApi also includes CrudApiHandler which implements the basic crud methods
``` csharp
        Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier);
        Task<IApiResponse<List<TResource>>> GetAsync();
        Task<IApiResponse<TResource>> CreateAsync(TResource resource);
        Task<IApiResponse> DeleteAsync(TIdentifier identifier);
        Task<IApiResponse<TResource>> ReplaceAsync(TResource resource);
        Task<IApiResponse<TResource>> UpdateAsync(TResource resource);
```
Below examples define how to use CrudApiHandler.

``` csharp
public interface IStudentApi: ICrudApi<StudentDto, long>
{
}
```
``` csharp
public class StudentApiHandler: CurdApiHandler<StudentDto, long>, IStudentApi
{
	public StudentApiHandler(IApiOptions<IStudentApi> options): base(options)
	{
	}
}
```


# Can I use different versions of my APIs

# Does SereneApi have a best practices guide?

# How does authentication work?
