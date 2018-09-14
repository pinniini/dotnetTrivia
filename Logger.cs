using System;
using System.Collections.Generic;

namespace dotnetTrivia
{
    public static class Logger
    {
        private static IList<string> _logs = new List<string>();

        public static void Log(string message)
        {
            // Just in case.
            if (_logs == null)
            {
                _logs = new List<string>();
            }

            if (message == null)
            {
                throw new ArgumentNullException("Message cannot be null.");
            }

            _logs.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}");
        }

        public static void PrintLog()
        {
            foreach (string message in _logs)
            {
                Console.WriteLine(message);
            }
        }
    }
}