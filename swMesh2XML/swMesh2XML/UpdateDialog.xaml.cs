using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace swMesh2XML
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : UserControl
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void GetUpdate_Btn_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://github.com/Lewinator56/swMesh2XML_repo/releases/latest",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            checkNewVersion();
        }

        public void SetUpdateAvailable(bool updateAvailable, string version)
        {
            if (updateAvailable)
            {
                GetUpdate_Btn.IsEnabled = true;
                UpdateInfo.Content = "A new Version is available\nVersion: " + GlobalVar.version + " -> Version: " + version;
            }
            else
            {
                UpdateInfo.Content = "You are already running the latest version";
            }
        }

        private void checkNewVersion()
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("https://github.com/Lewinator56/swMesh2XML_repo/releases/latest");
            wr.AllowAutoRedirect = true;
            HttpWebResponse wrs = (HttpWebResponse)wr.GetResponse();
            string onlineVer = wrs.ResponseUri.ToString().Substring(wrs.ResponseUri.ToString().LastIndexOf('/') + 1);

            if (onlineVer != GlobalVar.version)
            {
                GetUpdate_Btn.IsEnabled = true;
                UpdateInfo.Content = "A new Version is available\nVersion: " + GlobalVar.version + " -> Version: " + onlineVer;

            }
            else
            {
                UpdateInfo.Content = "You are already running the latest version";
            }

        }
    }
}
