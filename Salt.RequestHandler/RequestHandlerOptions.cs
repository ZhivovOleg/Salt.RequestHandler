namespace Salt.RequestHandler
{
	/// <summary>
	/// Options for handle requests
	/// </summary>
	public class RequestHandlerOptions
	{
		/// <summary>
		/// Policy - what to do
		/// </summary>
		public RequestHandlerPolicy RequestHandlerPolicy { get; set; }
		/// <summary>
		/// Directory, where requests save
		/// </summary>
		public string LogPath { get; set; }
	}
}
