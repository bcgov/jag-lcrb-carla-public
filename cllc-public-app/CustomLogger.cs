using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public
{

    public class CllcConsoleLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public int EventId { get; set; } = 0;
    }
    public class CllcConsoleLoggerProvider : ILoggerProvider
    {
        private readonly CllcConsoleLoggerConfiguration _config;
        private readonly IHostingEnvironment _env;
        private readonly ConcurrentDictionary<string, CllcConsoleLogger> _loggers = new ConcurrentDictionary<string, CllcConsoleLogger>();
        public CllcConsoleLoggerProvider(CllcConsoleLoggerConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new CllcConsoleLogger(name, _config, _env));
        }
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
    public class CllcConsoleLogger : ILogger
    {
        private static object _lock = new Object();
        private readonly string _name;
        private readonly CllcConsoleLoggerConfiguration _config;
        private readonly IHostingEnvironment _env;
        private readonly bool _production;

        public CllcConsoleLogger(string name, CllcConsoleLoggerConfiguration config, IHostingEnvironment env)
        {
            _name = name;
            _config = config;
            _env = env;

            if (_env != null && _env.IsProduction())
            {
                _production = true;
            }
            else
            {
                _production = false;
            }

        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            lock (_lock)
            {
                //filter log messages here (created to filter health check logs)
                if ((_config.EventId == 0 || _config.EventId == eventId.Id)
                    && (state != null && !state.ToString().Contains("/hc"))
                    && ((state != null && !state.ToString().Contains("200 application/json") || !_production)))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {logLevel.ToString()}:  {_name}:\n\t {formatter(state, exception)}");
                }
            }
        }
    }
}
