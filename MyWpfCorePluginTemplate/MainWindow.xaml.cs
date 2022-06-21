using MyWpfCorePluginTemplate.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using PluginContractsDll;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System;
using MyWpfCorePluginTemplate.Dialogs;

namespace MyWpfCorePluginTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// ToolBar Png from:
    /// https://www.flaticon.com
    /// and we need:
    /// Install-Package NLog
    /// Install-Package System.ComponentModel.Composition
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static NLog.Logger _nlogger = NLog.LogManager.GetCurrentClassLogger();
        AboutBoxDlg _aboutBoxDlg = new AboutBoxDlg();

        const string PLUGIN_DIR = "\\Plugins";
        PluginRepository PluginRepository { get; set; }

        #region INotify Changed Properties
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }

        private ObservableCollection<PluginServiceNameId> pluginList;
        public ObservableCollection<PluginServiceNameId> PluginList
        {
            get { return pluginList; }
            set { SetField(ref pluginList, value, nameof(PluginList)); }
        }
        private PluginServiceNameId selectedPlugin;
        public PluginServiceNameId SelectedPlugin
        {
            get { return selectedPlugin; }
            set 
            { 
                SetField(ref selectedPlugin, value, nameof(SelectedPlugin));
                try
                {
                    // ToDo
                    // This is just for testing only. A universal solution must be implemented.
                    string serviceName = "";
                    serviceName = PluginList.ToList().Find(x => x.PluginServiceName == selectedPlugin.PluginServiceName).Id;
                    PluginView = PluginRepository.PluginServiceList.Find(v => v.ToString() == serviceName);
                    Message = $"Plugin {serviceName} was loaded";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Message = "Plugin not found";
                }
            }
        }

        private PluginService.IPluginService pluginView;
        public PluginService.IPluginService PluginView
        {
            get { return pluginView; }
            set { SetField(ref pluginView, value, nameof(PluginView)); }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Title += Globals.BuildString;

            PluginList = new ObservableCollection<PluginServiceNameId>();

            LoadPlugins();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        /// <summary>
        /// MenuItem_Exit_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// MenuItem_Tool1_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Tool1_Click(object sender, RoutedEventArgs e)
        {
            Message = "MenuItem_Tool1_Click";
            MessageBox.Show("You pressed MenuItem 1 or Toolbar Button 1", Path.GetFileNameWithoutExtension(Globals.Exe), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// MenuItem_Tool2_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Tool2_Click(object sender, RoutedEventArgs e)
        {
            Message = "MenuItem_Tool2_Click";
            MessageBox.Show("You pressed MenuItem 2 or Toolbar Button 2", Path.GetFileNameWithoutExtension(Globals.Exe), MessageBoxButton.OK,MessageBoxImage.Information);
        }

        /// <summary>
        /// MenuItem_DeleteLogFiles_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_DeleteLogFiles_Click(object sender, RoutedEventArgs e)
        {
            Directory.GetFiles(Globals.ExeDir, "*.log", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
            Message = $"All log files have been deleted in {Globals.ExeDir}";
        }

        /// <summary>
        /// MenuItem_Help_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            Message = "MenuItem_Help_Click";
            System.Windows.Forms.Help.ShowHelp(null, "MyWpfCorePluginTemplate.chm");
        }

        /// <summary>
        /// MenuItem_ChangeLog_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ChangeLog_Click(object sender, RoutedEventArgs e)
        {
            Message = "View ChangeLog";
            ChangeLogCoreUtilityDll.ChangeLogTxtToolWindow ChangeLogTxtToolWindow = new ChangeLogCoreUtilityDll.ChangeLogTxtToolWindow(this);
            ChangeLogTxtToolWindow.ShowChangeLogWindow("ChangeLog.txt");
        }

        /// <summary>
        /// MenuItem_About_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            Message = "MenuItem_About_Click";
            _aboutBoxDlg.ShowAboutBoxDlg($"AboutBox of {Path.GetFileNameWithoutExtension($"{Globals.Exe}")}");
        }

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Status_MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Status_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Message = "";
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _aboutBoxDlg.Close();
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// LoadPlugins
        /// </summary>
        private void LoadPlugins()
        {
            try
            {
                // We set the Plugin-Directory under the directory where the application is located
                string pluginDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + PLUGIN_DIR;
                var catalog = new DirectoryCatalog(pluginDir);
                var container = new CompositionContainer(catalog);
                PluginRepository = new PluginRepository(container.GetExportedValues<PluginContractsDll.PluginService.IPluginService>());

                foreach (var service in PluginRepository.PluginServiceList)
                {
                    string retCode = service.InitPlugin(); // <-- call the special plugin method
                    Debug.WriteLine("Load Plugin " + service.ToString() + " RetCode: " + retCode);
                    PluginList.Add(new PluginServiceNameId { Ord = service.Ord, Id = service.ToString(), PluginServiceName = retCode });
                }

                // Reorder the PluginList
                PluginList = new ObservableCollection<PluginServiceNameId>(PluginList.OrderBy(p => p.Ord));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// SetField
        /// for INotify Changed Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

    #region Help classes
    public class PluginServiceNameId
    {
        public int Ord { get; set; }
        public string Id { get; set; }
        public string PluginServiceName { get; set; }
    }
    public class PluginRepository
    {
        [ImportMany("IPluginService")]
        private IEnumerable<PluginContractsDll.PluginService.IPluginService> PluginServices { get; set; }
        // For more convenience we will copy the available plugins in a List<>
        public List<PluginContractsDll.PluginService.IPluginService> PluginServiceList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pluginServices"></param>
        public PluginRepository(IEnumerable<PluginContractsDll.PluginService.IPluginService> pluginServices)
        {
            PluginServiceList = new List<PluginService.IPluginService>();
            foreach (var service in pluginServices)
                PluginServiceList.Add(service);
        }
    }
    #endregion

}
