using System;
using System.Reflection;

namespace TrayMonitor
{
    public static class AppInfo
    {
        static AppInfo() {
            var assembly = Assembly.GetAssembly(typeof(AppInfo));
            Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            Version = (assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version) ?? "";
            CommandLine = Environment.CommandLine;
        }

        public static string CommandLine { get; }

        public static string Version { get; }

        public static string Title { get; }
    }
}