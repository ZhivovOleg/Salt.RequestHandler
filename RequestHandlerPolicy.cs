namespace Salt.RequestHandler;

/// <summary>
/// Possible policies for handler
/// </summary>
public enum RequestHandlerPolicy
{
    /// <summary>
    /// Handler do nothing
    /// </summary>
    NoAction = 0,
    /// <summary>
    /// Handler logging only those request, which workflow returns exception
    /// </summary>
    HandleOnlyCrashed = 1,
    /// <summary>
    /// Handler log all requests
    /// </summary>
    HandleAll = 2
}