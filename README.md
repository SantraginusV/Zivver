- [READMEs are just suggestions anyways](#readme)
- :unicorn: [Application details](#application-details)
	- [Prerequisites](#prerequisites) :heavy_check_mark:
	- [Installation and running the app](#installation-and-running-the-app) :running:
	- [Troubleshooting](#troubleshooting) :fire_engine:
- [Q&A](#qa-section)
***

# README
*Remember, a few hours of trial and error can save you several minutes of looking at the README.*


# Application details
## Prerequisites

- .Net Core 3.1

## Installation and running the app

Application can be published from Visual Studio. Inside folder `bin\publish\` file named Zivver.exe will be created to run the app.

The app can also be run directly from within Visual Studio.

## Troubleshooting

For logging, I've used SeriLog. File is stored in `Logs` folder. The path can be changed in App.xaml.cs.


# Q&A section

1. In C# there are several ways to make code run in multiple threads. To make things easier, the await keyword was introduced; what does this do? 
&nbsp;

	Threads are actual OS threads with multiple workers available inside for executing tasks. A CPU core can normally only run 1 thread at the time. Changing context (from one thread to another) is an expensive operation. 

	A task can be described as an abstraction of threads, like one of the workers in a thread. By default, Tasks are created in a thread pool, which is responsible for scheduling, counting and managing those tasks. If there are too many tasks, the pool will wait for some to execute and finish or it will create a new thread.
	&nbsp;
	
	The .NET framework introduced `await` and `async` for us to just focus on tasks; these are created in the background and set us free from managing threads, providing the best performance and avoid blocking execution of our code.

	When using asynchronous programming obvious pitfalls can be deadlocks and race conditions. 
	&nbsp;

	Here's pseudo code to explain how `await` works:


```C#
public async Task DontBlockMeAsyncMethod()
{
	// Here we start execution of our method SomeApiCall() 
	// in the background as a new task in the thread pool
	Task task = SomeApiCall();

	{ some work to do }

	// Here we need the data from the method SomeApiCall()
	// We ask for the result and `await` if not ready
	int taskResult = await task();
	// This will not block the thread, 
	// and the caller can continue its execution in parallel

	{ do something with the taskResult }
}
```
&nbsp;
***


2. If you make http requests to a remote API directly from a UI component, the UI will freeze for a while, how can you use await to avoid this and how does this work?
&nbsp;

&emsp;&emsp;If calling directly from GUI object:

~~~~ C#
Dispatcher.Invoke(() => { SomeAPIWork });
~~~~

&emsp;&emsp;If calling from methods that are in ViewModels (for example: UpdateButton_ClickAsync):

```C#
await Task.Run(() => { SomeAPIWork });
```

&emsp;&emsp;This will run on a thread from the pool instead of UI thread, which means our UI thread won't be blocked and our app won't freeze.
&nbsp;
***


3. Imagine that you have to process a large unsorted CSV file with two columns: productId (int) and availableIn (ISO2 String, e.g. "US", "NL"). The goal is to group the file sorted by productId together with a list where the product is available. Example: 1, "DE" 2, "NL" 1, "US" 3, "US" Becomes: 1 -> ["DE", "US"] 2 -> ["NL"] 3 -> ["US"]

	a. How would you do this using LINQ syntax (write a short example)?  
```C#
var result = file.GroupBy(
    f => f.productId, f => f.availableIn, (id, langs) => new {
	productId = id,
	availableIn = langs.ToList()
});
```
&nbsp;

&emsp;&emsp;b. The program crashes with an OutOfMemoryError after processing approx. 80%. What would you do to succeed?

&emsp;&emsp;I would limit amount of bytes read at a time, and read file in blocks instead a whole file at once.

&nbsp;
***

4. In C# there is an interface IDisposable.  
  a. Give an example of where and why to implement this interface.  
  b. We can use disposable objects in a using block. What is the purpose of doing this?
&nbsp;

	a. Even though C# deallocates memory on it's own with garbage collector when a resource is not used anymore, there are some resources that garbage collector is not aware of. 
	Those resources are called unmanaged resources and some of them are file handlers, network connections/sockets, database connections. Since GC is not aware of them, we need to use Dispose for memory deallocation.
	
	 For example, if we would not dispose database connection, db connection pool would run out of available connections because we are not releasing it, and trying to create a new connection would end up with exception. It's similar with network connections. 
&nbsp;

	b. Using block makes sure Dispose() is called at the end of execution of the block, and using is doing next:
		
```C#
try 
	{ some work with unmanaged resources }
finally
	{ object.Dispose(); }
```
&nbsp;
***



5. When a user logs in on our API, a JWT token is issued and our Outlook plugin uses this token for every request for authentication. Here's an example of such a token:
> eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFt
> ZSI6IkplcmVteSIsImFkbWluIjpmYWxzZX0.BgcLOiwBvyuisQk9yWW0q0ZScMyI
> HNmDctw12-meCHU

&emsp;&emsp;Why and when is it (or isn't it) safe to use this?
&nbsp;

JWT are basically JSON objects that can be used to securely transmit information between two endpoints (i.e., client and server).

A little bit of background: JSON Web Token (JWT) is a de facto standard which is still in proposal (Request for Comments) phase.
JWT is an easy and fast to decode token that consists of a Header, the Payload, and a Verification Signature.

First, we can differentiate two "types" of JWTs:
* The first one (and most common use) is to securely transmit account sessions or access tokens.
* The second method can be used to send (secret) information between two endpoints. To achieve that, we need to make sure the JWTs are also encrypted.

For this answer, we're gonna focus on using the issued JWT to use for every request (as part of authentication).

The issued JWT for our Outlook plugin will allow our regular user Jeremy, based upon his subscription, grant access to the API.

> Why and when is it (or isn't it) safe to use this?

In this scenario, we assume session management is handled by an endpoint, checking, and authorizing the plugin to access (specific) API routes.

If this assumption is correct, I think it makes sense and is safe enough to use the token in this manner. The token contains the information the API needs to validate Jeremy's session, has access to the necessary data elements and is more compact than XML. In case of multi-domain APIs, JWTs will also be preferable as it doesn't rely on cookies (and therefore is CORS-proof).


However, I would want to propose to add some additional information to the token:

JWT works with claims, allowing us to have more granular control over the token itself. Some of these predefined claims will help us validate and verify the token better.

My suggestion would be to add the following:
* exp - expiration time to ensure the JWT becomes invalid after this specific datetime
* iat - issued at to determine the age of the JWT, allowing us to invalidate the token based upon age.
* sub - subject to optionally add a stricter scope for access to parts of the API.


We should assure that session state is not directly stored / maintained in the JWT as we don't want to solely rely on the (client-side) token. It would potentially be a vulnerability if the session state is part of the token.

We should also assure the connection between two endpoints is always secure as the JWT is openly transferred.

We could also consider adding refresh tokens to add an extra layer of security, whereas the normal JWT has a relatively short lifespan, and the refresh token is used to request a new normal JWT after expiration. If the refresh token has expired, we can redirect the user to authenticate again.
