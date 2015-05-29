#if !__MonoCS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace Marea
{
    /// <summary>
    /// Interaction logic for MareaGui.xaml
    /// </summary>
    public partial class MareaGui : Window
    {
        public ServiceContainer container;

        private Thread refreshThr;

        public MareaGui(ServiceContainer container)
        {
            this.container = container;
            Resources["AvailableServices"] = this.container.serviceManager.GetSDUs();
            lock (this.container.serviceManager.runningServices)
                Resources["RunningServices"] = this.container.serviceManager.runningServices;
            lock (this.container.serviceManager.proxies)
                Resources["RemoteServices"] = this.container.serviceManager.proxies;

            InitializeComponent();
            DisableStartStopButtons();

            refreshThr = new Thread(RefreshRemoteListView);
            refreshThr.Start();
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableLstView.SelectedItem != null)
            {

                if ((bool)NodeInfoCkeckBox.IsChecked)
                {
                    IPAddress ipAddress = IPAddress.Parse(IPTextBox.Text);
                    int port = Convert.ToInt32(PortTextBox.Text);
                    IPEndPoint ipEP = new IPEndPoint(ipAddress, port);

                    container.serviceManager.StartService(container.defaultSubsystem, ipEP, InstanceTextBox.Text, ((KeyValuePair<string, ServiceImplementation>)availableLstView.SelectedItem).Key);
                }
                else
                    container.serviceManager.StartService(((KeyValuePair<string, ServiceImplementation>)availableLstView.SelectedItem).Key);
                lock (runningLstView)
                    runningLstView.Items.Refresh();
            }

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (runningLstView.SelectedItem != null)
            {
                lock (runningLstView)
                {
                    container.serviceManager.StopService(((KeyValuePair<MareaAddress, IService>)runningLstView.SelectedItem).Key);
                    runningLstView.Items.Refresh();
                }
                //Resources["RunningServices"] = this.container.services.services.Where(kvp => ((Service)kvp.Value).isStarted);
            }
        }

        #region GUI_Behaviuor
        private void DisableStartStopButtons()
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = false;

        }
        private void avaliableLstView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            runningLstView.SelectedItem = -1;

        }

        private void runningLstView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            availableLstView.SelectedItem = -1;
        }

        private void NodeInfoCkeckBox_Checked(object sender, RoutedEventArgs e)
        {
            IPTextBox.Text = container.defaultNode.Address.ToString();
            PortTextBox.Text = Convert.ToString(container.defaultNode.Port);
            InstanceTextBox.Text = "0";
            PortTextBox.IsEnabled = true;
            IPTextBox.IsEnabled = true;
            InstanceTextBox.IsEnabled = true;
        }

        private void NodeInfoCkeckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PortTextBox.Text = "";
            IPTextBox.Text = "";
            InstanceTextBox.Text = "";
            PortTextBox.IsEnabled = false;
            IPTextBox.IsEnabled = false;
            InstanceTextBox.IsEnabled = false;
        }

        private void RefreshRemoteListView()
        {
            while (true)
            {
                DispatcherOperation op = Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                      new Action<ListView>(RefreshItemsFromListView), remoteLstView);

                while (op.Status != DispatcherOperationStatus.Completed)
                {
                    Thread.Sleep(1000);

                }
            }
        }

        private void RefreshItemsFromListView(ListView listView)
        {
            lock (remoteLstView)
                remoteLstView.Items.Refresh();
            lock (runningLstView)
                runningLstView.Items.Refresh();
        }

        #endregion
    }
}

#endif