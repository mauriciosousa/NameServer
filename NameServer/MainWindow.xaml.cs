using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NameServer
{
    public class Host
    {
        public string id { get; set; }
        public string address { get; set; }
        public string port { get; set; }
        public Host(string id, string address, string port)
        {
            this.id = id;
            this.address = address;
            this.port = port;
        }
    }


    public partial class MainWindow : Window
    {
        private List<Host> _hosts;
        private NSTcpListener _tcpListener;

        public MainWindow()
        {
            InitializeComponent();
            _hosts = new List<Host>();
            

            try
            {
                _tcpListener = new NSTcpListener(57743, _hosts);
                _tcpListener.NewNotification += new NotificationEventHandler(notificationArrived);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            loadTable();
        }

        private void loadTable()
        {
            _hosts = TableFile.Load("table.txt");
            lvHosts.ItemsSource = _hosts;
        }

        public void addHost(string id, string address, string port)
        {
            if (!alreadyRegistered(id))
            {
                _hosts.Add(new Host(id, address, port));
            }
        }

        public void writeConsole(string s)
        {
                textBlock.Text = textBlock.Text + "\n " + s;
                Scroller.ScrollToBottom();
        }

        private void notificationArrived(object sender, string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                writeConsole(message);
            });
        }

        public bool alreadyRegistered(string id)
        {
            foreach (Host h in _hosts)
            {
                if (h.id == id) return true; 
            }
            return false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_tcpListener != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    _tcpListener.Close();
                });
            }
            Environment.Exit(0);
        }

        private void Reload_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            loadTable();
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Window_Closed(sender, e);
        }
    }
}
