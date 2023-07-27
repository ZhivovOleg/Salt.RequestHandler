namespace Salt.RequestHandler;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

/// <summary>
/// Middleware extension
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Add Salt.RequestHandler to middleware
    /// </summary>
    /// <param name="app">app</param>
    /// <param name="handlerPolicy">what handler have to do</param>
    /// <param name="logPath">Null, if you want to log only with ILogger</param>
    /// <returns></returns>
    public static IApplicationBuilder AddRequestHandler(
        this IApplicationBuilder app,
        RequestHandlerPolicy handlerPolicy,
        string logPath = null)
    {
        app.UseMiddleware<RequestHandlerMiddleware>(Options.Create(new RequestHandlerOptions { RequestHandlerPolicy = handlerPolicy, LogPath = logPath }));
        return app;
    }
}