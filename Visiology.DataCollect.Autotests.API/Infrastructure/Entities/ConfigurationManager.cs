using Microsoft.Extensions.Configuration;

namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities
{
    public class ConfigurationManager
    {
        private readonly IConfiguration _configuration;

        public ConfigurationManager()
        {
            var builder = new ConfigurationBuilder().AddXmlFile("App.config");
            this._configuration = builder.Build();
        }

        public string GetValue(string key)
        {
            return  this._configuration[$"appSettings:{key}"];
        }
    }
}
