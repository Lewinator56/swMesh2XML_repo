using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Diagnostics;

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
            opf.Filter = "Stormworks mesh files (.mesh)|*.MESH|XML file (.xml)|*.xml|Wavefront file (.obj)|*.obj";
            Nullable<bool> r = opf.ShowDialog();
            if (r == true)
            {
                if (System.IO.Path.GetExtension(opf.FileName) == ".mesh")
                {
                    openMesh(opf.FileName);
                }
                else if (System.IO.Path.GetExtension(opf.FileName) == ".obj")
                {
                    openObj(opf.FileName);
                } else
                {
                    openXml(opf.FileName);
                }
            }
        }

        private void openMesh(string filePath)
        {
            inTextBox.Text = "";
            byte[] f = File.ReadAllBytes(filePath);
            bin = f;
            int nlc = 0;
            for (int i = 0; i < f.Length; i++)
            {

                if (nlc == 15)
                {
                    nlc = 0;
                    inTextBox.Text += Environment.NewLine;
                }
                inTextBox.Text += f[i].ToString("X2") + " ";
                nlc++;
            }
            toMesh.IsEnabled = false;
            toXML.IsEnabled = true;
            this.Title = "swMesh2XML : MESH : " + filePath;
        }
        private void openXml(String filePath)
        {
            inTextBox.Text = File.ReadAllText(filePath);
            this.Title = "swMesh2XML : XML : " + filePath;
            toMesh.IsEnabled = true;
            toXML.IsEnabled = false;
        }
        private void openObj(String filePath)
        {
            inTextBox.Text = File.ReadAllText(filePath);
            this.Title = "swMesh2XML : OBJ : " + filePath;
            toMesh.IsEnabled = true;
            toXML.IsEnabled = false;
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
            int docIt = 0;

            XmlWriter xw = XmlWriter.Create("temp.xml", xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("mesh");
            // 6D 65 73 68 07 00 01 00
            // Vertex count
            UInt16 vc = (UInt16)(bin[9] << 8 | bin[8]);
            xw.WriteElementString("vertex_count", vc.ToString());
            // 13 00
            docIt += 14;

            xw.WriteStartElement("vertices");
            
            List<vertex> vl = new List<vertex>();
            for (int i = 0; i < vc; i++)
            {

                byte[] by = bin[(docIt)..(docIt+28)];
                Single px = BitConverter.ToSingle(by[0..4], 0);
                Single py = BitConverter.ToSingle(by[4..8]);
                Single pz = BitConverter.ToSingle(by[8..12]);
                byte r = by[12];
                byte g = by[13];
                byte b = by[14];
                byte a = by[15];
                Single nx = BitConverter.ToSingle(by[16..20]);
                Single ny = BitConverter.ToSingle(by[20..24]);
                Single nz = BitConverter.ToSingle(by[24..28]);
                vertex v = new vertex(px, py, pz, r, g, b, a, nx, ny, nz);
                vl.Add(v);
                xw.WriteStartElement("vertex");


                xw.WriteAttributeString("pos", px + " " + py + " " + pz);
                xw.WriteAttributeString("color", r + " " + g + " " + b + " " + a);
                xw.WriteAttributeString("normal", nx + " " + ny + " " + nz);
                xw.WriteEndElement();
                docIt += 28;
                //126
                
            }

            xw.WriteEndElement();
            // edges
            UInt32 eb = BitConverter.ToUInt32(bin[docIt..(docIt + 4)]);
            xw.WriteElementString("edge_buffer", eb.ToString());
            // 00 00 
            UInt32 tc = (eb * 2)/6 ;
            docIt += 4;
            xw.WriteStartElement("triangles");
            for (int i = 0; i < tc; i++)
            {
                byte[] by = bin[(docIt)..(docIt + 6)];
                docIt += 6;
                UInt16 p1 = BitConverter.ToUInt16(by[0..2]);
                UInt16 p2 = BitConverter.ToUInt16(by[2..4]);
                UInt16 p3 = BitConverter.ToUInt16(by[4..6]);

                xw.WriteStartElement("triangle");
                xw.WriteAttributeString("vertices", p1 + " " + p2 + " " + p3);
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            //Materials
            xw.WriteStartElement("sub-meshes");
            UInt16 smc = BitConverter.ToUInt16(bin[docIt..(docIt + 2)]);
            xw.WriteAttributeString("number", smc + "");
            docIt += 2;
            for (int i = 0; i < smc; i++)
            {
                xw.WriteStartElement("sub-mesh");
                UInt32 sms = BitConverter.ToUInt32(bin[docIt..(docIt + 4)]);
                docIt += 4;
                UInt32 sme = BitConverter.ToUInt32(bin[docIt..(docIt + 4)]);
                docIt += 6; // skip a 00 00 pad
                UInt16 sid = BitConverter.ToUInt16(bin[docIt..(docIt + 2)]);
                docIt += 2;
                xw.WriteAttributeString("start_index", sms + "");
                xw.WriteAttributeString("end_index", sme + "");
                xw.WriteAttributeString("shader_id", sid + "");

                // culling
                Single cmxx = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                Single cmxy = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                Single cmxz = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                Single cmnx = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                Single cmny = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                Single cmnz = BitConverter.ToSingle(bin[docIt..(docIt + 4)]);
                docIt += 4;
                xw.WriteStartElement("culling_area");
                xw.WriteAttributeString("min", cmxx + " " + cmxy + " " + cmxz);
                xw.WriteAttributeString("max", cmnx + " " + cmny + " " + cmnz);
                xw.WriteEndElement();
                xw.WriteEndElement();
                docIt += 2;
                UInt16 idl = BitConverter.ToUInt16(bin[docIt..(docIt + 2)]);
                docIt += idl;
                docIt += 14;
            }
            xw.WriteEndElement();

            

            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
            outTextBox.Text = File.ReadAllText("temp.xml");
        }

        private void xmlToMesh()
        {

        }
        private void objToMesh()
        {

        }

        private void toMesh_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
