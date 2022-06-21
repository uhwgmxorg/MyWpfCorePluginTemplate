using System.Windows;

namespace MyWpfCorePluginTemplate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static NLog.Logger _nlogger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            _nlogger.Info($"START {System.AppDomain.CurrentDomain.FriendlyName}");
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        /// <summary>
        /// OnExit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            _nlogger.Info($"EXIT {System.AppDomain.CurrentDomain.FriendlyName}");
            base.OnExit(e);
        }

        /// <summary>
        /// OnDispatcherUnhandledException
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An unhandled exception occurred\nin {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}:\n{e.Exception.Message}";
            _nlogger.Error($"Exception: {errorMessage}");
            _nlogger.Error($"Exception ToString(): {e.ToString()}");
            MessageBox.Show($"{errorMessage}\nSource:\n{e.Exception.Source}\n\nLook in the Log-File for more information !", $"Error  in  {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
