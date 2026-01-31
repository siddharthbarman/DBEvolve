using Microsoft.Extensions.Logging;

namespace SByteStream.DBEvolve
{
    sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }

    public class SimpleFileLogger : ILogger
    {
        public SimpleFileLogger(string name, LogLevel level)
        {
            Name = name;
            Level = level;
        }

        public string Name { get; private set; }

        public LogLevel Level { get; set; }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel < Level) return;

            string message = formatter(state, exception);
            string ts = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string? level = Enum.GetName(typeof(LogLevel), logLevel)?.ToUpper().Substring(0, 4);

            if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (logLevel == LogLevel.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            
            Console.WriteLine($"{ts} [{level}] [{Name}]: {message}");
            Console.ResetColor();

            g_streamWriter.WriteLine($"{ts} [{level}] [{Name}]: {message}");            

            if (exception != null)
            {                
                Console.WriteLine(exception);
                g_streamWriter.WriteLine(exception);            
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel >= Level);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        private static StreamWriter g_streamWriter = new StreamWriter("dbmaint.log", false) { AutoFlush = true };

    }
}
