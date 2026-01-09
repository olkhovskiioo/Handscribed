using Handscribed.DataLoader;
using Handscribed.Dataset;
using Handscribed.TrainDataViewer;
using HandyControl.Tools.Extension;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Handscribed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = ((TrainDataViewerViewModel)TrainDataViewer.DataContext);
            if (!string.IsNullOrEmpty(vm.FolderName))
                trainTab.IsEnabled = true;
            else
                ((TrainDataViewerViewModel)TrainDataViewer.DataContext).DataSelected += MainWindow_DataSelected;

            mainTabControl.SelectedIndex = Properties.Settings.Default.SelectedTab;
        }

        private void MainWindow_DataSelected(object? sender, EventArgs e)
        {
            trainTab.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((TrainDataViewerViewModel)TrainDataViewer.DataContext).DataSelected -= MainWindow_DataSelected;
            ((TrainDataViewerViewModel)TrainDataViewer.DataContext).Save();
            Properties.Settings.Default.SelectedTab = mainTabControl.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
