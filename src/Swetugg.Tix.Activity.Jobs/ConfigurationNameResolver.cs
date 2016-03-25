using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Swetugg.Tix.Activity.Jobs
{
    public class ConfigurationNameResolver : INameResolver
    {
        private readonly IConfigurationRoot _config;

        public ConfigurationNameResolver(IConfigurationRoot config)
        {
            this._config = config;
        }

        public string Resolve(string name)
        {
            return _config[name];
        }
    }
}