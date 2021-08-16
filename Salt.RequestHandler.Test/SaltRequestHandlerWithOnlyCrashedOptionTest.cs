using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace Salt.RequestHandler.Test
{
    [TestFixture]
    public class SaltRequestHandlerWithOnlyCrashedOptionTest
    {
        private IHost _host;
        // project directory
        private string _projectDirPath = Directory.GetParent(Directory
            .GetParent(Directory.GetParent(Directory
                .GetCurrentDirectory()).FullName).FullName).FullName;

        [SetUp]
        public async Task Setup()
        {
             _host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                    //services.AddHttpContextAccessor();
                    //services.AddLogging(logger => 
                    //{
                    //    logger.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "Salt.RequestsHandler.Test.log"));
                    //});
                })
                .ConfigureLogging((builder, logger) =>
                {
                    logger.AddFile(Path.Combine(_projectDirPath, "Salt.RequestsHandler.Test.log"));
                })
                .Configure(app =>
                { 
                    app.AddRequestHandler(RequestHandlerPolicy.HANDLE_ONLY_CRASHED, Path.Combine(_projectDirPath, "RequestHandles"));
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
        })
        .StartAsync();
        }

        [Test]
        public async Task RequestHandler_ForPostRequest_AreReturnHttpResponseStatus500()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();

            server.BaseAddress = new System.Uri("http://localhost:9000/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestAction/");
            object obj = new 
            { 
                date = "2021-08-13T09:09:06.401Z", 
                temperatureC = 0, 
                summary = "string" 
            };
            string content = JsonConvert.SerializeObject(obj);
            StringContent stringContent = new(content, UnicodeEncoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, stringContent);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public void RequestHandler_ForPostRequest_DoesNotThrowException()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();
            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestAction/");
            object obj = new
            {
                date = "2021-08-13T09:09:06.401Z",
                temperatureC = 0,
                summary = "string"
            };
            string content = JsonConvert.SerializeObject(obj);
            StringContent stringContent = new(content, UnicodeEncoding.UTF8, "application/json");

            Assert.DoesNotThrowAsync(() => client.PostAsync(client.BaseAddress, stringContent));
        }

        [Test]
        public void RequestHandler_ForPostRequestWithNullBody_DoesNotThrowException()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();

            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");

            Assert.DoesNotThrowAsync(() => client.PostAsync(client.BaseAddress, null));
        }

        [Test]
        public async Task RequestHandler_ForPostRequest_RequestHandlesDirectoryLengthAreIncremented()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();
            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestAction/");
            object obj = new
            {
                date = "2021-08-13T09:09:06.401Z",
                temperatureC = 0,
                summary = "string"
            };
            string content = JsonConvert.SerializeObject(obj);
            StringContent stringContent = new(content, UnicodeEncoding.UTF8, "application/json");
            DirectoryInfo dir = new(Path.Combine(_projectDirPath, "RequestHandles"));
            int dirOldLength = dir.GetFiles().Length;

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, stringContent);
            int dirLength = dir.GetFiles().Length;

            Assert.That(dirLength > dirOldLength);
        }

        [Test]
        public async Task RequestHandler_ForGetRequest_AreReturnHttpResponseStatus500()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();
            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestActionGet/");

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public void RequestHandler_ForGetRequest_DoesNotThrowException()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();

            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestActionGet/");

            Assert.DoesNotThrowAsync(() => client.GetAsync(client.BaseAddress));
        }

        [Test]
        public async Task RequestHandler_ForGetRequest_RequestHandlesDirectoryLengthAreIncremented()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();
            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/TestActionGet/");
            DirectoryInfo dir = new(Path.Combine(_projectDirPath, "RequestHandles"));
            int dirOldLength = dir.GetFiles().Length;

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            int dirLength = dir.GetFiles().Length;

            Assert.That(dirLength > dirOldLength);
        }

        [Test]
        public async Task RequestHandler_ForGetSuccessRequest_RequestHandlesDirectoryLengthDoesNotIncremented()
        {
            HttpClient client = _host.GetTestClient();
            TestServer server = _host.GetTestServer();
            server.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            client.BaseAddress = new System.Uri("http://localhost:9000/WeatherForecast/");
            DirectoryInfo dir = new(Path.Combine(_projectDirPath, "RequestHandles"));
            int dirOldLength = dir.GetFiles().Length;

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            int dirLength = dir.GetFiles().Length;

            Assert.That(dirLength == dirOldLength);
        }

    }
}