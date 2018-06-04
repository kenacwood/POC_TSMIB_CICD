using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TriTech.Common;

namespace TritechDecryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XMLReader();

        }

        private void XMLReader()

        {
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(MultiEnvironmentList));
            // StreamReader streamReader = File.OpenText(Environment.CurrentDirectory + "\\LaunchConfiguration.xml");


            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "TriTech.config")); //Combines the location of App_Data and the file name
            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "LaunchInstructions - Interface.xml")); //Combines the location of App_Data and the file name
            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "LaunchInstructions - VisiCAD.xml")); //Combines the location of App_Data and the file name
            //note encrypted =  XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "LaunchConfiguration.xml")); //Combines the location of App_Data and the file name
            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "New_TriTech.config")); //Combines the location of App_Data and the file name
            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "New_Clear_LaunchInstructions - VisiCAD.xml")); //Combines the location of App_Data and the file name
            //XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "New_Clear_LaunchInstructions - Interface.xml")); //Combines the location of App_Data and the file name

            XmlTextReader reader = new XmlTextReader(System.IO.Path.Combine(Environment.CurrentDirectory, "LaunchInstructions.xml")); //Combines the location of App_Data and the file name

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        listBox1.Items.Add("<" + reader.Name + ">");
                        if (reader.HasAttributes)
                        {
                            listBox1.Items.Add("Attributes of <" + reader.Name + ">");

                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "key")
                                        listBox1.Items.Add(String.Format(" {0}={1}", reader.Name, reader.Value));
                                    if (reader.Name == "value")
                                        listBox1.Items.Add(String.Format(" {0}={1}", reader.Name, Cryptography.Decrypt(reader.Value, "Europa")));
                                }
                            }

                            // Move the reader back to the element node.
                            reader.MoveToElement();
                        }
                        break;
                    case XmlNodeType.Text:
                        listBox1.Items.Add(Cryptography.Decrypt(reader.Value, "Europa"));
                        break;
                    case XmlNodeType.Attribute:
                        listBox1.Items.Add(Cryptography.Decrypt(reader.Value, "Europa"));
                        break;
                    case XmlNodeType.EndElement:
                        listBox1.Items.Add("");
                        break;
                }
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {

            //DecryptTriTechConfig();

            DecryptLaunchConfigs("LaunchInstructions - Interface.xml");

            DecryptLaunchConfigs("LaunchInstructions - VisiCAD.xml");



        }
        private void NewTriTechConfig()
        {

            string configFile = System.IO.Path.Combine(Environment.CurrentDirectory, "Clear_TriTech.config");
            XDocument xmlDoc = XDocument.Load(configFile);

            var nodes = from item in xmlDoc.DescendantNodes() select item;
            foreach (XNode node in nodes)

            {
                if (node is XElement)
                {
                    //listBox1.Items.Add((node as XElement).Name);
                    if ((node as XElement).Name == "add")
                    {

                        string value = (node as XElement).Attribute("value").Value;
                        (node as XElement).SetAttributeValue("value", Cryptography.Encrypt(value, "Europa"));
                    }
                }



            }
            xmlDoc.Save("New_TriTech.config");
        }
        private void  DecryptTriTechConfig()
        {

            string configFile = System.IO.Path.Combine(Environment.CurrentDirectory, "TriTech.config");
            XDocument xmlDoc = XDocument.Load(configFile);

            var nodes = from item in xmlDoc.DescendantNodes() select item;
            foreach (XNode node in nodes)

            {
                if (node is XElement)
                {
                    //listBox1.Items.Add((node as XElement).Name);
                    if ((node as XElement).Name == "add")
                    {
                        string value = (node as XElement).Attribute("value").Value;

                        (node as XElement).SetAttributeValue("value", Cryptography.Decrypt(value, "Europa"));
                    }
                }

                //else
                //    listBox1.Items.Add(node);

            }
            xmlDoc.Save("Clear_TriTech.config");
        }

        private void DecryptLaunchConfigs(string fileName)
        {

            string configFile = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            XDocument xmlDoc = XDocument.Load(configFile);

            var nodes = from item in xmlDoc.DescendantNodes() select item;
            foreach (XNode node in nodes)

            {
                if (node is XElement)
                {
                    XElement element = (XElement)node;
                    if (!element.HasElements && !element.IsEmpty)
                    {
                        string value = Cryptography.Decrypt(element.Value, "Europa") ;
                        element.SetValue(value);

                       //
                    }


                }



            }
            xmlDoc.Save("Clear_" + fileName);
        }
        private void EncryptCleanLaunchConfigs(string fileName)
        {

            string configFile = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            XDocument xmlDoc = XDocument.Load(configFile);

            var nodes = from item in xmlDoc.DescendantNodes() select item;
            foreach (XNode node in nodes)

            {
                if (node is XElement)
                {
                    XElement element = (XElement)node;
                    if (!element.HasElements && !element.IsEmpty)
                    {
                        string value = Cryptography.Encrypt(element.Value, "Europa");
                        element.SetValue(value);
                    }


                }



            }
            xmlDoc.Save("New_" + fileName);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            NewTriTechConfig();


            EncryptCleanLaunchConfigs("Clear_LaunchInstructions - Interface.xml");

            EncryptCleanLaunchConfigs("Clear_LaunchInstructions - VisiCAD.xml");
        }
    }
}
