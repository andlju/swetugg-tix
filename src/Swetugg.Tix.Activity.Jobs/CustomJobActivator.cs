using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace Swetugg.Tix.Activity.Jobs
{
    public class CustomJobActivator : IJobActivator
    {
        private readonly IServiceProvider _service;
        public CustomJobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            var service = _service.GetService<T>();
            return service;
        }
    }
}