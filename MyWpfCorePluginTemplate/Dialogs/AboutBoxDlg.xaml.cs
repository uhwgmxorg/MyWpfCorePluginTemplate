using MyWpfCorePluginTemplate.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MyWpfCorePluginTemplate.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBoxDlg : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _desktopWorkingAreaRight = 0.0;
        private double _desktopWorkingAreaBottom = 0.0;

        private string aboutBox;
        public string AboutBox
        {
            get { return aboutBox; }
            set { SetField(ref aboutBox, value, nameof(AboutBox)); }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }
        private double transparents;
        public double Transparents
        {
            get { return transparents; }
            set { SetField(ref transparents, value, nameof(Transparents)); }
        }
        private Brush backgrount1;
        public Brush Backgrount1
        {
            get { return backgrount1; }
            set { SetField(ref backgrount1, value, nameof(Backgrount1)); }
        }
        private Brush backgrount2;
        public Brush Backgrount2
        {
            get { return backgrount2; }
            set { SetField(ref backgrount2, value, nameof(Backgrount2)); }
        }
        private Color color1;
        public Color Color1
        {
            get { return color1; }
            set { SetField(ref color1, value, nameof(Color1)); }
        }
        private Color color2;
        public Color Color2
        {
            get { return color2; }
            set { SetField(ref color2, value, nameof(Color2)); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutBoxDlg()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ShowAboutBoxDlg(string text = "")
        {
            AboutBox = text;
            Transparents = 1;
            Color1 = StringToColor("#FFE8BF9D");
            Color2 = StringToColor("#FFEFD6C1");
            Backgrount1 = new SolidColorBrush(Color1);
            Backgrount2 = new SolidColorBrush(Color2);
            Message = Globals.BuildString.Trim();
            Message += "\npress Left-Mouse-Button to close";
            Message += "\npress Right-Mouse-Button to move window";
            Message += "\nuse Mouse-Wheel for opacity";
            Show();
        }

        public void HideAboutBoxDlg()
        {
            Hide();
        }

        private void Window_Main_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            _desktopWorkingAreaRight = desktopWorkingArea.Right;
            _desktopWorkingAreaBottom = desktopWorkingArea.Bottom;

            CenterWindow(this, _desktopWorkingAreaRight, _desktopWorkingAreaBottom);
        }

        private void Window_Main_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();
        }

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            double d = (double)e.Delta / 1000;
            double o = Transparents;

            o -= d;
            if (0.2 <= o && o <= 1)
                Transparents = o;
            Debug.WriteLine($"Transparents={Transparents} d={d} o={o}");
        }

        private void CenterWindow(Window win, double desktopWorkingAreaRight, double desktopWorkingAreaBottom)
        {
            if (win == null || desktopWorkingAreaRight < 0 || desktopWorkingAreaBottom < 0)
                return;
            win.Left = (desktopWorkingAreaRight / 2 + win.Width / 2) - win.Width;
            win.Top = (desktopWorkingAreaBottom / 2 + win.Height / 2) - win.Height;
        }

        private Color StringToColor(string colorString)
        {
            Color c = new Color();
            if (colorString.StartsWith("#"))
            {
                colorString = colorString.Replace("#", "");
                byte a = System.Convert.ToByte("ff", 16);
                byte pos = 0;
                if (colorString.Length == 8)
                {
                    a = System.Convert.ToByte(colorString.Substring(pos, 2), 16);
                    pos = 2;
                }
                byte r = System.Convert.ToByte(colorString.Substring(pos, 2), 16);
                pos += 2;
                byte g = System.Convert.ToByte(colorString.Substring(pos, 2), 16);
                pos += 2;
                byte b = System.Convert.ToByte(colorString.Substring(pos, 2), 16);
                c = Color.FromArgb(a, r, g, b);
                return c;
            }

            return c;
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
