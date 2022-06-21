namespace MyWpfCorePluginTemplate.Tools
{
    public static class Globals
    {
        public static string Revison { get; } = "0";
        public static string ExeDir
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        public static string Exe
        {
            get
            {
                return $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.exe";
            }
        }
        public static string BuildString 
        {
            get
            {
                System.DateTime buildDate = System.IO.File.GetLastWriteTime(System.IO.Path.Combine(ExeDir, Exe));
#if DEBUG
                return $"    Debug Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}  Revison {Globals.Revison} - Build-Date {buildDate}";
#else
                return $"    Release Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}  Revison {Globals.Revison} - Build-Date {buildDate}";
#endif
            } 
        }
    }
}
