using Microsoft.Extensions.Configuration;
using System;

namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Configuration
{
    public class ConfigurationManager
    {
        private readonly IConfiguration _configuration;

        public ConfigurationManager()
        {
            var builder = new ConfigurationBuilder().AddXmlFile("App.config");
            _configuration = builder.Build();
        }

        public string GetValue(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrEmpty(value))
            {
                return _configuration[$"appSettings:{key}"];
            }

            return value;            
        }
    }
}
