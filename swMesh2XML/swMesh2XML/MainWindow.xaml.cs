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
using Microsoft.Win32;

namespace swMesh2XML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[] bin;
        string fileType;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "Stormworks mesh files (.mesh)|*.MESH|XML file (.xml)|*.xml|Wavefront file (.obj)|*.obj";
            outTextBox.Width = double.NaN;
            outTextBox.TextWrapping = TextWrapping.NoWrap;
            
            Nullable<bool> r = opf.ShowDialog();
            if (r == true)
            {
                saveFile.IsEnabled = false;
                if (System.IO.Path.GetExtension(opf.FileName) == ".mesh")
                {
                    this.fileType = "mesh";
                    outTextBox.Clear();
                    openMesh(opf.FileName);
                    
                }
                else if (System.IO.Path.GetExtension(opf.FileName) == ".obj")
                {
                    this.fileType = "obj";
                    outTextBox.Clear();
                    openObj(opf.FileName);
                    toXML.IsEnabled = false;
                    
                } else
                {
                    showError("This isnt implemented yet, sorry");

                    //this.fileType = "xml";
                    //openXml(opf.FileName);
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
            toXML.IsEnabled = true;
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
                vertex v = new vertex(px, py, pz, r, g, b, a, new Normal(nx, ny, nz));
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
            saveFile.IsEnabled = true;
        }

        private void xmlToMesh()
        {

        }
        private void objToMesh(string[] data)
        {
            try
            {
                Debug.WriteLine(data.Length);
                UInt16 vertexCount = 0;
                Color c = new Color();
                c.R = 255;
                c.G = 125;
                c.B = 0;
                c.A = 255;
                int vtxPosCount = 0;
                List<Normal> normals = new List<Normal>();
                List<int> subMeshVertices = new List<int>();
                List<SubMesh> subMeshes = new List<SubMesh>();
                int curretSubmesh = -1;
                subMeshVertices.Add(0);
                for (int i = 0; i < data.Length; i++)
                {

                    if (data[i].StartsWith('#'))
                    {
                        // ignore
                    }
                    else if (data[i].StartsWith("mtllib"))
                    {
                        //ignore
                    }
                    else if (data[i].StartsWith('o'))
                    {
                        Debug.WriteLine("adding submesh");
                        if (data[i].Contains('.'))
                        {
                            string[] col = data[i].Split('.')[1].Split('-');
                            c.R = Convert.ToByte(col[0]);
                            c.G = Convert.ToByte(col[1]);
                            c.B = Convert.ToByte(col[2]);
                            c.A = Convert.ToByte(col[3]);

                        }
                        curretSubmesh++;
                        vtxPosCount = 0;
                        subMeshes.Add(new SubMesh());
                        subMeshVertices.Add(0);
                    }
                    else if (data[i].StartsWith('v') && !data[i].StartsWith("vt") && !data[i].StartsWith("vn"))
                    {
                        string[] v = data[i].Split(' ');
                        Single vx = Convert.ToSingle(v[1]);
                        Single vy = Convert.ToSingle(v[2]);
                        Single vz = Convert.ToSingle(v[3]);
                        vertex vtx = new vertex(vx, vy, vz, c.R, c.G, c.B, c.A);
                        subMeshes[curretSubmesh].addVertex(vtx);
                        subMeshes[curretSubmesh].setCullingMax(vx, vy, vz);
                        subMeshes[curretSubmesh].setCullingMin(vx, vy, vz);
                        vertexCount++;
                        vtxPosCount++;
                        subMeshVertices[curretSubmesh + 1]++;
                    }
                    else if (data[i].StartsWith("vt"))
                    {
                        // ignore
                    }
                    else if (data[i].StartsWith("vn"))
                    {
                        // todo change for normal matching
                        string[] n = data[i].Split(' ');
                        Single nx = Convert.ToSingle(n[1]);
                        Single ny = Convert.ToSingle(n[2]);
                        Single nz = Convert.ToSingle(n[3]);
                        normals.Add(new Normal(nx, ny, nz));
                        //subMeshes[curretSubmesh].vertices[vtxPosCount-1].setNormals(nx, ny, nz);
                    }
                    else if (data[i].StartsWith("usemtl"))
                    {
                        UInt16 shaderId = Convert.ToUInt16(data[i].Split(' ')[1]);
                        subMeshes[curretSubmesh].setShader(shaderId);
                    }
                    else if (data[i].StartsWith('s'))
                    {
                        // ignore
                    }
                    else if (data[i].StartsWith('f'))
                    {
                        string[] t = data[i].Split(' ');
                        int nidxv1 = Convert.ToInt32(t[1].Split('/')[2]) - 1;
                        int nidxv2 = Convert.ToInt32(t[2].Split('/')[2]) - 1;
                        int nidxv3 = Convert.ToInt32(t[3].Split('/')[2]) - 1;
                        byte v1 = (byte)(Convert.ToByte(t[1].Split('/')[0]) - 0x01);
                        byte v2 = (byte)(Convert.ToByte(t[2].Split('/')[0]) - 0x01);
                        byte v3 = (byte)(Convert.ToByte(t[3].Split('/')[0]) - 0x01);
                        Triangle trg = new Triangle(v1, v2, v3);
                        Debug.WriteLine("vertex " + v1 + " inn submesh " + curretSubmesh + "\\" + subMeshes.Count() + " normals " + nidxv1 + " of " + normals.Count());
                        subMeshes[curretSubmesh].vertices[v1 - subMeshVertices[curretSubmesh]].setNormals(normals[nidxv1]);
                        subMeshes[curretSubmesh].vertices[v2 - subMeshVertices[curretSubmesh]].setNormals(normals[nidxv2]);
                        subMeshes[curretSubmesh].vertices[v3 - subMeshVertices[curretSubmesh]].setNormals(normals[nidxv3]);

                        subMeshes[curretSubmesh].addTriangle(trg);
                    }
                }
                Debug.WriteLine(subMeshes.Count);
                // mesh writer

                // write header
                List<byte> mesh = new List<byte>();
                mesh.AddRange(new byte[] { 0x6D, 0x65, 0x73, 0x68, 0x07, 0x00, 0x01, 0x00 });
                // write vertex count
                mesh.AddRange(BitConverter.GetBytes(vertexCount));
                mesh.AddRange(new byte[] { 0x13, 0x00, 0x00, 0x00 });
                UInt32 tc = 0;
                foreach (SubMesh sm in subMeshes)
                {
                    Debug.WriteLine("looking at submesh");
                    foreach (vertex v in sm.vertices)
                    {
                        Debug.WriteLine("looking at vertex");
                        List<byte> vtxDef = new List<byte>();
                        vtxDef.AddRange(BitConverter.GetBytes(v.px));
                        vtxDef.AddRange(BitConverter.GetBytes(v.py));
                        vtxDef.AddRange(BitConverter.GetBytes(v.pz));
                        vtxDef.Add(v.r);
                        vtxDef.Add(v.g);
                        vtxDef.Add(v.b);
                        vtxDef.Add(v.a);
                        vtxDef.AddRange(BitConverter.GetBytes(v.n.x));
                        vtxDef.AddRange(BitConverter.GetBytes(v.n.y));
                        vtxDef.AddRange(BitConverter.GetBytes(v.n.z));
                        mesh.AddRange(vtxDef);
                    }
                    tc += Convert.ToUInt32(sm.triangles.Count());

                }

                mesh.AddRange(BitConverter.GetBytes(tc * 3));
                //mesh.AddRange(new byte[] { 0x00, 0x00 });
                foreach (SubMesh sm in subMeshes)
                {
                    foreach (Triangle t in sm.triangles)
                    {
                        List<byte> tDef = new List<Byte>();
                        tDef.AddRange(BitConverter.GetBytes(t.v1));
                        tDef.AddRange(BitConverter.GetBytes(t.v3));
                        tDef.AddRange(BitConverter.GetBytes(t.v2));
                        mesh.AddRange(tDef);
                    }
                }
                mesh.AddRange(BitConverter.GetBytes(Convert.ToUInt16(subMeshes.Count())));
                UInt32 subMeshPosition = 0;
                foreach (SubMesh sm in subMeshes)
                {

                    mesh.AddRange(BitConverter.GetBytes(subMeshPosition));
                    subMeshPosition += Convert.ToUInt32(sm.vertices.Count());
                    mesh.AddRange(BitConverter.GetBytes(sm.vertices.Count()));
                    mesh.AddRange(new byte[] { 0x00, 0x00 });
                    Debug.WriteLine(sm.shader);
                    mesh.AddRange(BitConverter.GetBytes(sm.shader));
                    foreach (Single s in sm.cullingMin)
                    {
                        mesh.AddRange(BitConverter.GetBytes(s));
                    }
                    foreach (Single s in sm.cullingMax)
                    {
                        mesh.AddRange(BitConverter.GetBytes(s));
                    }
                    mesh.AddRange(new byte[] { 0x00, 0x03, 0x00, 0x49, 0x44, 0x33, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F });
                }
                mesh.AddRange(new byte[] { 0x00, 0x00 });


                Debug.WriteLine(mesh.Count);
                //int nlc = 0;
                //for (int i = 0; i < mesh.Count; i++)
                //{

                //  if (nlc == 15)
                // {
                //   nlc = 0;
                //  outTextBox.Text += Environment.NewLine;
                //}
                //outTextBox.Text += mesh[i].ToString("X2") + " ";
                //nlc++;
                //}

                // debug
                int nlc = 0;
                string bs = BitConverter.ToString(mesh.ToArray());
                bs = bs.Replace('-', ' ');
                outTextBox.Text = bs;
                outTextBox.Width = 350;
                outTextBox.TextWrapping = TextWrapping.Wrap;
                bin = mesh.ToArray();
                saveFile.IsEnabled = true;
            }
            catch (Exception e)
            {
                ErrorPopup ep = new ErrorPopup();
                ep.errorText.Text = "Oops, Something went wrong! \nPlease check your object file is in the correct format and try again\n\n For the dev:\n" + e.ToString();
                MaterialDesignThemes.Wpf.DialogHost.Show(ep);
            }

        }

        private void toMesh_Click(object sender, RoutedEventArgs e)
        {
            if (fileType == "obj")
            {
                outTextBox.Clear();
                objToMesh(inTextBox.Text.Split("\n"));
            }
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = fileType == "obj" || fileType == "xml" ? "Stormworks Mesh File (.mesh)|*.mesh" : "XML File (.xml)|*.xml";
            if (sfd.ShowDialog() == true)
            {
                if (fileType == "obj")
                {
                    File.WriteAllBytes(sfd.FileName, bin);
                } else if (fileType == "mesh")
                {
                    File.WriteAllText(sfd.FileName, outTextBox.Text);
                }
            }
            
        }

        private void showError(string msg)
        {
            ErrorPopup ep = new ErrorPopup();
            ep.errorText.Text = msg;
            MaterialDesignThemes.Wpf.DialogHost.Show(ep);
        }
    }
}
