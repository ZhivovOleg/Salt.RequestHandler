namespace Salt.RequestHandler
{
	/// <summary>
	/// Possible policies for handler
	/// </summary>
	public enum RequestHandlerPolicy
	{
		/// <summary>
		/// Handler do nothing
		/// </summary>
		NO_ACTION = 0,
		/// <summary>
		/// Handler logging only those request, which workflow returns exception
		/// </summary>
		HANDLE_ONLY_CRASHED = 1,
		/// <summary>
		/// Handler log all requests
		/// </summary>
		HANDLE_ALL = 2
	}
}
