using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Web.Tests
{
    public abstract class BaseWebTests
    {
        protected TestServer _testServer;

        protected BaseWebTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Shop.Web.Startup>();

            _testServer = new TestServer(builder);
        }
    }
}