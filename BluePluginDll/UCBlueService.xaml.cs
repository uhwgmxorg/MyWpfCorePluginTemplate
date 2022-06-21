using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace BluePluginDll
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UCBlueService : UserControl, INotifyPropertyChanged, PluginContractsDll.PluginService.IPluginService
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // The order Number of the Plugin in the List
        public int Ord { get; set; }

        private string pluginPluginMessage;
        public string PluginMessage
        {
            get { return pluginPluginMessage; }
            set { SetField(ref pluginPluginMessage, value, nameof(PluginMessage)); }
        }

        public UCBlueService()
        {
            InitializeComponent();
            DataContext = this;

            PluginMessage = "The Blue Plugin";
        }

        public string InitPlugin()
        {
            // Order Number 2
            Ord = 2;
            return "#2 BluePluginDll";
        }

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
    }
}
