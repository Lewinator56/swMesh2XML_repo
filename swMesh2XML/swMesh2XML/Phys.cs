using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace swMesh2XML
{
    class Phys
    {
        public static string ToXml(byte[] file)
        {
            string outs = "";
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            xws.Encoding = Encoding.UTF8;
            int docIt = 0;

            XmlWriter xw = XmlWriter.Create("temp.xml", xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("phys");
            // 70 68 79 73 02 00
            // mesh count?
            UInt16 mc = BitConverter.ToUInt16(file[6..7]);
            // write this to the file
            xw.WriteElementString("submesh_count", mc.ToString());
            // set the iterator to 8 (we need this now)
            docIt += 8;

            // loop through each submesh
            for (int i = 0; i < mc; i++)
            {
                xw.WriteStartElement("submesh");
                UInt16 vc = BitConverter.ToUInt16(file[(docIt)..(docIt + 1)]);
                docIt += 2;
                xw.WriteElementString("vertex_count", vc.ToString());

                // loop through vertices in the submesh
                for (int j = 0; j < vc; j++)
                {
                    byte[] by = file[(docIt)..(docIt + 28)];
                    Single px = BitConverter.ToSingle(by[0..4], 0);
                    Single py = BitConverter.ToSingle(by[4..8]);
                    Single pz = BitConverter.ToSingle(by[8..12]);

                    xw.WriteStartElement("vertex");
                    xw.WriteAttributeString("pos", px + " " + py + " " + pz);
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();
                // cater for 00 padding
                docIt += 2;
            }

            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
            outs = File.ReadAllText("temp.xml");




            return outs;
        }

    }
}
