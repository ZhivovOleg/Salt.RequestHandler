# Salt.RequestHandler

## Description

> Middleware component for viewing the content (url, body, parameters) of incoming requests. Passes the request to the next middleware component with the ability re-reading. The result is written to the log or to the log and the specified directory in the form of a json file.

# Using

## Add new dependency to your project

go to the folder of your project and complete

```console

   dotnet add reference ../Salt.RequestHandler/Salt.RequestHandler.csproj

```

## Add RequestHandler to your app

in your Startup.cs:
```c#
//...
public class Startup
{
    //...
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        //...
        app.AddRequestHandler(RequestHandlerPolicy.HANDLE_ONLY_CRASHED, DirPath);
        //...
    }
}
//
```

### Params

* RequestHandlerPolicy 

    >Request handler level. See more on Configuration.

* string DirPath: (default=null)

    >You will be able to receive the contents of each request in a separate json file in this directory.

# Configure

## Request handler levels

* RequestHandlerPolicy.NO_ACTION

   >Does not output any information about requets.

* RequestHandlerPolicy.HANDLE_ONLY_CRASHED

   >Handles information only about crushed requests. if the request is destroyed it will return the response status 500. It will write a description of the error in the log.

* RequestHandlerPolicy.HANDLE_ALL

   >Handles information about all requests.

## Configure a folder for logging requests

Set the path of the request logging folder in your Startup.cs:
```c#
//...
public class Startup
{
    //...
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        //...
        string requestHandleFolder = "RequestsHandler";
        app.AddRequestHandler(RequestHandlerPolicy.HANDLE_ONLY_CRASHED, requestHandleFolder);
        //...
    }
}
//
```

# Example of an output file

## For successful request

```json

2021.08.12.20.32.28

/WeatherForecast
?id=6

{"date":"2021-08-12T17:30:50.817Z","temperatureC":0,"summary":"string"}

```

## Example crushed request at logger

```console

{"date":"2021-08-13T09:09:06.401Z","temperatureC":0,"summary":"string"} (ebf92b00)
2021-08-16T10:35:12.0073493+03:00 0HMB0H1GPS20S [ERR] It was Test Exception! (22a14481)
System.Net.Http.HttpRequestException: It was Test Exception!
   at SaltRequestHandler.Controllers.WeatherForecastController.TestAction()...

```