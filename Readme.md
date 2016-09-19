DeepStreamNet
=============

dotnet Client for [deepstream.io](https://deepstream.io)

##Builds
[![Build status](https://ci.appveyor.com/api/projects/status/aj8op4emvlivn7jx/branch/develop?svg=true)](https://ci.appveyor.com/project/schulz3000/deepstreamnet/branch/develop) [![Build Status](https://travis-ci.org/schulz3000/deepstreamNet.svg?branch=develop)](https://travis-ci.org/schulz3000/deepstreamNet)

##Usage

```csharp
var client = new DeepStreamClient("localhost", 6021);

if(await client.LoginAsync())
{
 //Login was successful   
}

//Alternative with credentials
await client.LoginAsync("Username", "Password");

// Close Connection to deepstream.io endpoint
client.Dispose();
```

###Events

```csharp
// Subscribe to Event 'test'
var eventSubscription = await client.Events.Subscribe("test", x => { Console.WriteLine(x); });

// Send 'Hello' to all Subscribers of Event 'test'
await client.PublishAsync("test", "Hello");

// Send number '42' to all Subscribers of Event 'test'
await client.PublishAsync("test", 42);

// Send object '{Property1="Hello", Property2=42}' to all Subscribers of Event 'test'
await client.PublishAsync("test", new {Property1="Hello", Property2=42});

// Unsubscribe from Event 'test'
await eventSubscription.DisposeAsync();
```

###Records

```csharp
var record = await client.Records.GetRecordAsync("test");

record["FirstName"] = "John";
record["Age"] = 28;

// Save all changes
await client.Records.SaveAsync(record);

// Discard all changes
await client.Records.DiscardAsync(record);

// Delete Records
await client.Records.DeleteAsync(record);
```

###Dynamic Records

```csharp
dynamic record = await client.Records.GetRecordAsync("test");

record.FirstName = "Jane";
record.Age = 21;

// Save all changes
await client.Records.SaveAsync(record);
```

###RPC Request

```csharp
//Request RemoteProcedure 'toLowerCase' with argument 'abc'
var result = await client.Rpcs.Request<string,string>("toLowerCase", "abc");
//result == "ABC"
```

###RPC Provider

```csharp
//Define Method for RemoteProcedure
async Task ToUpperCase(string input, IRpcResponse<string> response)
{
    if (string.IsNullOrEmpty(input))
        await response.Error("input must not be empty");

    if (input == "ABC")
        await response.Reject();

    await response.Send(input.ToUpper());
}

//Register RemoteProcedure 'toUpperCase' with InputArgs as string and Result as string
var proc = await client.Rpcs.Register<string, string>("toUpperCase", ToUpperCase);

//alternative with anonymous Method as RemoteProcedure
var proc = await client.Rpcs.Register<string, string>("toUpperCase", (input)=>input.ToUpper());

//Unregister RemoteProcedure
await proc.DisposeAsync(); 
```