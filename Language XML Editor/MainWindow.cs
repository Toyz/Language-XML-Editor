using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Language_XML_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private readonly ObservableCollection<Models.ListData> _listDatas = new ObservableCollection<Models.ListData>(); 
        public MainWindow()
        {
            InitializeComponent();

            using (var wc = new WebClient())
            {
                wc.DownloadStringCompleted += WcOnDownloadStringCompleted;
                wc.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/PFCKrutonium/Windows-10-Login-Background-Changer/master/LangaugeLibrary/Langs/en_us.xml"));
            }

            ListViewXml.ItemsSource = _listDatas;
        }

        private void WcOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            var xml = XElement.Parse(downloadStringCompletedEventArgs.Result);

            foreach (var node in xml.Elements())
            {
                _listDatas.Add(new Models.ListData(node.Name.ToString(), node.Value));
            }
        }

        private void MenuOpenItem_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Language File (*.xml)|*.xml",
                Title = "Select Language File",
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                var xml = XElement.Parse(File.ReadAllText(ofd.FileName));
                LoadXml(xml.Elements());
            }
        }

        private void MenuSaveItem_OnClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Language File (*.xml)|*.xml",
                Title = "Save Language File"
            };

            if (sfd.ShowDialog() == true)
            {
                XmlToString(sfd.FileName);

                MessageBox.Show("Created language file", "Done");
            }
        }

        private void XmlToString(string file)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Language");

                foreach (var item in _listDatas)
                {
                    writer.WriteElementString(item.Name, item.Body);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void LoadXml(IEnumerable<XElement> elements)
        {
            List<string> edited = new List<string>();
            foreach (var node in elements)
            {
                var s =
                    (from current in _listDatas where current.Name.Equals(node.Name.ToString()) select current)
                        .FirstOrDefault();

                Debug.WriteLine(s);
                if (s != null)
                {
                    edited.Add(s.Name);
                    s.Body = node.Value;
                    s.BaseColor = new SolidColorBrush(Colors.White);
                        
                }
            }

            foreach (var item in _listDatas.Where(item => !edited.Contains(item.Name)))
            {
                item.BaseColor = new SolidColorBrush(Colors.LightCoral);
            }
        }
    }
}
