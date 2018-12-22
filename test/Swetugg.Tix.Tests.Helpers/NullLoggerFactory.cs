using System;
using Microsoft.Extensions.Logging;

namespace Swetugg.Tix.Tests.Helpers
{
    public class NullLoggerFactory : ILoggerFactory
    {
        class NullLogger : ILogger, IDisposable
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {

            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public void Dispose()
            {

            }
        }

        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new NullLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            
        }
    }
}