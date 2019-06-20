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
        private readonly ConcurrentDictionary<string, CllcConsoleLogger> _loggers = new ConcurrentDictionary<string, CllcConsoleLogger>();
        public CllcConsoleLoggerProvider(CllcConsoleLoggerConfiguration config)
        {
            _config = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new CllcConsoleLogger(name, _config));
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
        public CllcConsoleLogger(string name, CllcConsoleLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            lock (_lock)
            {
                if ((_config.EventId == 0 || _config.EventId == eventId.Id) && !state.ToString().Contains("cannabislicensing/hc"))
                {
                    Console.WriteLine($"{_name}:{logLevel.ToString()}:- {formatter(state, exception)}");
                }
            }
        }
    }
}
