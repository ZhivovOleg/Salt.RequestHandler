# Salt.RequestHandler

# Using

in your Startup.cs:
```c#
//...
public class Startup
{
    //...
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        //...
        app.AddRequestHandler(RequestHandlerPolicy.HANDLE_ONLY_CRASHED);
        //...
    }
}
//
```