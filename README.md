# NJsonApiCore
The .NET server implementation of the {**json:api**} standard running on .NET Core 1.0 (aka ASP.NET 5, MVC 6, DNX/vNext/OWIN) and .NET 4.5.2 MVC 5.

> This library is not a complete implementation of the JSONApi 1.0.

## Current Development Effort
NJsonApiCore supports .NET 4.6.2 and .NET Core 1.0.

## History
Originally courtesy of [**SocialCee**](http://socialcee.com), forked NJsonApi from the work done by https://github.com/jacek-gorgon/NJsonApi and then formed into its own repository courtesy of [**My Clinical Outcomes**](http://www.myclinicaloutcomes.com).

## How to use

### 1. Install
For your ASP.NET 4.5.2/MVC 5 project, install using nuget:

`Install-Package NJsonApiCore`

### 2. Create your NJsonApiCore configuration
You need to tell NJsonApiCore which controllers serve which resources. Create a satic C# class with a single method that returns `NJsonApi.IConfiguration`. The method will build up your resource configuration using a fluid API:

```cs
public static class MyNJsonApiConfiguration {
  public static IConfiguration Build()
  {
    var configBuilder = new ConfigurationBuilder();
    configBuilder
      .Resource<MyResource, MyResourceController>()
      .WithAllProperties();
    return configBuilder.Build();
  }
}
```

For .NET 4.6.2, see the MVC5 HelloWorld project for [an example](https://github.com/brainwipe/NJsonApiCore/blob/master/src/NJsonApiCore.Web.MVC5.HelloWorld/MyNJsonApiConfigurationBuilder.cs).

For .NET Core, see the MVC Core HelloWorld project for [an example](https://github.com/brainwipe/NJsonApiCore/blob/master/src/NJsonApiCore.Web.MVCCore.HelloWorld/NJsonApiConfiguration.cs)

### 3. ASP.NET 4.6.2: Update Application_Start() 
If you are using ASP.NET 4.6.2/MVC5 then you need to update your `Application_Start()` method in Global.asax.cs. The bare minimum you will need is:

```cs
protected void Application_Start()
{
    // Setup the dependency injection container
    var container = new UnityContainer();
    DependencyResolver.SetResolver(new UnityDependencyResolver(container));

    // Remove the generic XML formatter
    GlobalConfiguration.Configuration.Formatters.Clear();

    // Register the routes
    GlobalConfiguration.Configure(WebApiConfig.Register);

    // Build your NJsonApi configuration and pass it into the JsonApiAppStart configure method
    var jsonApiConfiguration = MyNJsonApiConfiguration.Build();
    JsonApiAppStart.Configure(GlobalConfiguration.Configuration, container, jsonApiConfiguration);
}
```

### 3. ASP.NET Core: Update Startup.cs
If you are using ASP.NET Core 1.0 then you need to update your `Startup.cs` class in two places.

```cs
public Startup(IHostingEnvironment env)
{
    var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
    Configuration = builder.Build();
}

public IConfigurationRoot Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();

    services.AddMvc(
        options =>
        {
            options.Conventions.Add(new ApiExplorerVisibilityEnabledConvention());
            options.Filters.Add(typeof(JsonApiActionFilter));
            options.Filters.Add(typeof(JsonApiExceptionFilter));
            options.OutputFormatters.Insert(0, new JsonApiOutputFormatter(nJsonApiConfig));
        });
   
    services.AddSingleton<ILinkBuilder, LinkBuilder>();
    services.AddSingleton(nJsonApiConfig.GetJsonSerializer());
    services.AddSingleton<IJsonApiTransformer, JsonApiTransformer >();
    services.AddSingleton(nJsonApiConfig);
    services.AddSingleton<TransformationHelper>();
}
```

## Example of use
There are two example projects in this repository, one for [API.NET 4.5.2/MVC 5](https://github.com/brainwipe/NJsonApiCore/tree/master/src/NJsonApiCore.Web.MVC5.HelloWorld) and one for [.NET Core](https://github.com/brainwipe/NJsonApiCore/blob/master/src/NJsonApiCore.Web.MVCCore.HelloWorld). A solution file is included for each.

## Example
Load the *NJsonApiCore.Web.MVC5.HelloWorld* solution and run the HelloWorld project. It runs under IISExpress. You can then send requests to the NJsonApi using a REST client, such as [Postman](https://chrome.google.com/webstore/detail/postman/fhbjgbiflinjbdggehcddcbncdddomop?hl=en).

The example HelloWorld projects both implement the entities found on the [JSONApi homepage](http://jsonapi.org/). 

```cs
public class Article
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Person> Author { get; set; }
    public List<Comment> Comments { get; set}
}

public class Person
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Twitter { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
}
```

A GET request to `http://localhost:5000/articles/1?include=comments.people` gives the compound document:

```json
{
  "data": {
    "id": "1",
    "type": "articles",
    "attributes": {
      "title": "JSON API paints my bikeshed!"
    },
    "relationships": {
      "author": {
        "links": {
          "self": "http://localhost:5000/articles/1/relationships/author",
          "related": "http://localhost:5000/articles/1/author"
        },
        "data": {
          "id": "3",
          "type": "people"
        }
      },
      "comments": {
        "links": {
          "self": "http://localhost:5000/articles/1/relationships/comments",
          "related": "http://localhost:5000/articles/1/comments"
        },
        "data": [
          {
            "id": "5",
            "type": "comments"
          },
          {
            "id": "6",
            "type": "comments"
          }
        ],
        "meta": {
          "count": "2"
        }
      }
    },
    "links": {
      "self": "http://localhost:5000/articles/1"
    }
  },
  "links": {
    "self": "http://localhost:5000/articles/1?include=comments.people"
  },
  "included": [
    {
      "id": "3",
      "type": "people",
      "attributes": {
        "firstName": "Dan",
        "lastName": "Gebhardt",
        "twitter": "dgeb"
      },
      "links": {
        "self": "http://localhost:5000/people/3"
      }
    },
    {
      "id": "5",
      "type": "comments",
      "attributes": {
        "body": "First!"
      },
      "relationships": {
        "author": {
          "links": {
            "self": "http://localhost:5000/comments/5/relationships/author",
            "related": "http://localhost:5000/comments/5/author"
          },
          "data": {
            "id": "3",
            "type": "people"
          }
        }
      },
      "links": {
        "self": "http://localhost:5000/comments/5"
      }
    },
    {
      "id": "6",
      "type": "comments",
      "attributes": {
        "body": "I like XML Better"
      },
      "relationships": {
        "author": {
          "links": {
            "self": "http://localhost:5000/comments/6/relationships/author",
            "related": "http://localhost:5000/comments/6/author"
          },
          "data": {
            "id": "4",
            "type": "people"
          }
        }
      },
      "links": {
        "self": "http://localhost:5000/comments/6"
      }
    },
    {
      "id": "4",
      "type": "people",
      "attributes": {
        "firstName": "Rob",
        "lastName": "Lang",
        "twitter": "brainwipe"
      },
      "links": {
        "self": "http://localhost:5000/people/4"
      }
    }
  ],
  "jsonapi": {
    "Version": "1.0"
  }
}
```
