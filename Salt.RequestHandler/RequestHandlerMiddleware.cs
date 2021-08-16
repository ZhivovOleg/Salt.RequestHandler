using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Salt.RequestHandler
{
	/// <summary>
	/// Middleware for advanced exceptions handling
	/// </summary>
	public class RequestHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestHandlerMiddleware> _logger;
		private readonly RequestHandlerOptions _options; 

		/// <summary>
		/// ctor
		/// </summary>
		public RequestHandlerMiddleware(
			RequestDelegate next, 
			ILogger<RequestHandlerMiddleware> logger,
			IOptions<RequestHandlerOptions> options)
		{
			_next = next;
			_logger = logger;
			_options = options.Value;
		}
		
		private async Task SaveRequest(HttpRequest request)
		{
			StringBuilder recordSb = new();
			string currentTime = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");
			
			try
			{
				recordSb.Append(Environment.NewLine);
				recordSb.AppendLine(currentTime);
				recordSb.Append(Environment.NewLine);
				recordSb.AppendLine(request.Path);
				recordSb.Append(request.QueryString);
				recordSb.Append(Environment.NewLine);

				if (request.ContentLength is > 0)
					using (StreamReader reader = new(
						request.Body,
						encoding: Encoding.UTF8,
						detectEncodingFromByteOrderMarks: false,
						bufferSize: Convert.ToInt32(request.ContentLength),
						leaveOpen: true))
					{
						request.Body.Position = 0;
						string body = await reader.ReadToEndAsync();
						recordSb.Append(Environment.NewLine);
						recordSb.Append(body);
						request.Body.Position = 0; // Reset the request body stream position so the next middleware can read it
					}
			}
			catch (Exception exc)
			{
				_logger.LogError($"Error on read request: {exc.Message}", exc);
				throw;
			}
			
			try
			{
				_logger.LogInformation(recordSb.ToString());
				if (!string.IsNullOrEmpty(_options.LogPath))
				{
					if (!Directory.Exists(_options.LogPath))
						Directory.CreateDirectory(_options.LogPath);
					File.WriteAllText(Path.Combine(_options.LogPath, $"request_{request.GetHashCode()}_{currentTime}.json"), recordSb.ToString(), Encoding.UTF8);
				}
			}
			catch (Exception exc)
			{
				_logger.LogError($"Error on writing request to file: {exc.Message}", exc);
				throw;
			}
		}

		/// <summary>
		/// Basic middleware method
		/// </summary>
		public async Task Invoke(HttpContext context)
		{
			switch (_options.RequestHandlerPolicy)
			{
				case RequestHandlerPolicy.HANDLE_ALL:
					context.Request.EnableBuffering(); //enable re-reading requests
					await SaveRequest(context.Request);
					await _next.Invoke(context);
					break;
				case RequestHandlerPolicy.HANDLE_ONLY_CRASHED:
					context.Request.EnableBuffering(); //enable re-reading requests
					try
					{
						await _next.Invoke(context);
					}
					catch (Exception ex)
					{
						await SaveRequest(context.Request);
						_logger.LogError(ex, ex.Message);
						context.Response.StatusCode = 500;
						context.Response.ContentType = "application/text";
						await context.Response.WriteAsync(ex.Message);
					}
					break;
				case RequestHandlerPolicy.NO_ACTION: 
					await _next.Invoke(context);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
