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
using System.IO;
using System.Xml;

namespace swMesh2XML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[] bin;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "Stormworks mesh files (.mesh)|*.MESH|XML file (.xml)|*.xml";
            Nullable<bool> r = opf.ShowDialog();
            if (r == true)
            {
                if (System.IO.Path.GetExtension(opf.FileName) == ".mesh")
                {
                    openMesh(opf.FileName);
                }
                else
                {

                }
            }
        }

        private void openMesh(string filePath)
        {
            binaryTextBox.Text = "";
            byte[] f = File.ReadAllBytes(filePath);
            bin = f;
            int nlc = 0;
            for (int i = 0; i < f.Length; i++)
            {

                if (nlc == 15)
                {
                    nlc = 0;
                    binaryTextBox.Text += Environment.NewLine;
                }
                binaryTextBox.Text += f[i].ToString("X2") + " ";
                nlc++;
            }
        }

        private void ToXML_Click(object sender, RoutedEventArgs e)
        {
            if (bin != null)
            {
                convertToXML();
            }
            else
            {
                ErrorPopup ep = new ErrorPopup();
                ep.errorText.Text = "Please open a mesh file first";
                MaterialDesignThemes.Wpf.DialogHost.Show(ep);
            }
        }

        private void convertToXML()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            xws.Encoding = Encoding.UTF8;

            XmlWriter xw = XmlWriter.Create("temp.xml", xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("mesh");
            // Vertex count
            UInt16 vc = (UInt16)(bin[9] << 8 | bin[8]);
            xw.WriteElementString("vertex_count", vc.ToString());
            List<byte[]> vl = new List<byte[]>();
            for (int i = 0; i < vc; i++)
            {
                vl.Add(bin[(14 + (i * 28))..(14 + (14 + ((i + 1) * 28)))]);
            }




            xw.WriteEndElement();
            xw.Flush();
            xw.Close();
            xmlTextBox.Text = File.ReadAllText("temp.xml");
        }
    }
}
