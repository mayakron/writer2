using System;
using System.Windows;

namespace Writer2
{
    public partial class App : Application
    {
        public const string Version = "1.9.0";

        public static readonly string StartupDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
    }
}