using ConsoleApp.Abstractions;
using System;

namespace ConsoleApp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleAppBuilder(args).Build().Run();
        }

        public static IConsoleAppBuilder<Application> ConsoleAppBuilder(string[] args) =>
            ConsoleApp.DefaultBuilder<Application>(args);
    }
}
