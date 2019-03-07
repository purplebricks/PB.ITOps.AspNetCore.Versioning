[![Build status](https://ci.appveyor.com/api/projects/status/pb1gwc02bi153ln8?svg=true)](https://ci.appveyor.com/project/ilivewithian/pb-itops-aspnetcore-versioning)
[![NuGet](https://img.shields.io/nuget/v/PB.ITOps.AspNetCore.Versioning.svg)](https://www.nuget.org/packages/PB.ITOps.AspNetCore.Versioning/)

# ASP.NET API Versioning

This is an opinionated convention for versioning web APIs. It extends [Microsoft ASP.Net API Versioning](https://github.com/Microsoft/aspnet-api-versioning) by introducing a new convention and attributes.

## The convention

- There is only 1 supported API version at any given time (the latest). All other versions are deprecated.
- Only numbered major versions allowed (e.g `V1`, `V2`, `V3`, etc.)
- When a new API version is introduced, all actions are automatically added to the new API version (unless explicitly marked as removed).
  - Simplified api developer story - we only need to make changes where there are differences between versions. Previously we would need to manually add attributes to multiple classes and methods that have not had any changes.
  - Less error prone - Less chance of making a mistake when introducing a new api version.

# Getting Started

> This does not detail how to install swagger with api versioning, this is however installed in the example project. [More information can be found here](https://github.com/Microsoft/aspnet-api-versioning/wiki/API-Documentation).

> This is a demonstration on how to use the library, it may not follow best practice. Generally speaking we bump the api version when we introduce a breaking change.

## Configure API Versioning

Install nuget package to your asp.net core project...
  > Todo once on Nuget

Configure api versioning for V1 in `Startup.cs`
- Override the default conventions with `IntroducedApiVersionConventionBuilder`

```c#
    services.AddApiVersioning(options =>
    {
        options.Conventions = new IntroducedApiVersionConventionBuilder(1, 1);
    });
```

## Version a Controller

Decorate a controller with `[IntroducedInApiVersion(1)]`
- This means that this controller and all of it's actions will be available in V1 of the API unless they are explicitly introduced in another version, or removed.
- We can have many controllers intrduced in the same api version.
  
```c#
    // For demonstration purposes we will use uri versioning
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(1)]
    public class ValuesController : ControllerBase
    {
        // GET api/v1/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }
    }
```

## Add a new api version

Say we want to increase the api versions (so have V1 deprecated and V2 supported), we can achieve this by updating the second argument to `IntroducedApiVersionConventionBuilder` in `Startup.cs`:

```c#
    services.AddApiVersioning(options =>
    {
        // options.Conventions = new IntroducedApiVersionConventionBuilder(1, 1);
        options.Conventions = new IntroducedApiVersionConventionBuilder(1, 2);
    });
```

Our method in `ValuesController` is now in both v1 and v2.

```c#
    // GET api/v1/values (deprecated)
    // GET api/v2/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] {"value1", "value2"};
    }
```

We can add a new method to this controller, adding the `[IntroducedInApiVersion(2)` attribute. This action will only be available in v2 onwards (but not v1).

```c#
    // GET api/v2/values/123
    [HttpGet("{id}")]
    [IntroducedInApiVersion(2)]
    public ActionResult<string> Get(int id)
    {
        return $"{id}";
    }
```
We can also add another controller, and introduce this in v2.
```c#
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(2)]
    public class AnotherController : ControllerBase
    {
        // GET api/v2/another
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET api/v2/another/5
        [HttpGet("{id}")]
        [RemovedAsOfApiVersion(3)]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
```
## Removing a controller from an api version

Let's add a new api version by updating the second argument to `IntroducedApiVersionConventionBuilder` in `Startup.cs`:

```c#
    services.AddApiVersioning(options =>
    {
        // options.Conventions = new IntroducedApiVersionConventionBuilder(1, 2);
        options.Conventions = new IntroducedApiVersionConventionBuilder(1, 3);
    });
```
We now have 3 versions
- V1 (deprecated)
- V2 (deprecated)
- V3 (supported)

Let us remove `ValuesController` from V3. We do this 
```c#
    // For demonstration purposes we will use uri versioning
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(1)]
    [RemovedAsOfApiVersion(3)]
    public class ValuesController : ControllerBase
    {
        // GET api/v1/values
        // GET api/v2/values
        // Doesn't exist in v3
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET api/v2/values/123
        // Doesn't exist in v3
        [HttpGet("{id}")]
        [IntroducedInApiVersion(2)]
        public ActionResult<string> Get(int id)
        {
            return $"{id}";
        }
    }
```

## Removing an action from an api version

Let's say we make a breaking change, say some business rule has changed and ids now have some sort of prefix.

We can mark the old action as `RemovedAsOfApiVersion(3)` and introduce a new action that supports this changed requirement with the `IntroducedInApiVersion(3)`

```c#
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(2)]
    public class AnotherController : ControllerBase
    {
        // Other methods not shown for clarity

        // GET api/v2/another/5
        [HttpGet("{id}")]
        [RemovedAsOfApiVersion(3)]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // GET api/v3/another/x-5
        [HttpGet("{id}")]
        [IntroducedInApiVersion(3)]
        public ActionResult<string> Get(string id)
        {
            return "value";
        }
    }
```

## Removing an entire version

Imagine nobody is using V1 of our API anymore. We can easily remove it by updating `IntroducedApiVersionConventionBuilder` in `Startup.cs`:

```c#
    services.AddApiVersioning(options =>
    {
        // options.Conventions = new IntroducedApiVersionConventionBuilder(1, 3);
        options.Conventions = new IntroducedApiVersionConventionBuilder(2, 3);
    });
```
Now any routes with `api/v1/` have been removed.
Any code that is marked with attribute `RemovedAsOfApiVersion(3)` can be deleted.

# Example

Checkout the example project [Todo: Add Link to Github]() to see how this all works, and feel free to ask any questions.

## Problems running the example

### Https Redirection

If you are having trouble running the example with https on your local dev machine, make sure you have trusted the dev certificate.

Run from CLI: `dotnet dev-certs https --trust`

[More information in Microsoft docs](https://docs.microsoft.com/en-gb/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.2&tabs=visual-studio#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos).

# Common Questions

## Is ASP.net WebAPI supported?

At the moment only ASP.Net Core is supported. If there is demand we can look at adding this - I don't *think* it would be too difficult.


