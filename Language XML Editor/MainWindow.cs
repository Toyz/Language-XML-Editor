using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows.Controls;

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
                var nameS = node.Name.ToString().Split('_');
                var nameC = string.Join(" ", nameS);
                nameC = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameC.ToLower());
                _listDatas.Add(new Models.ListData(node.Name.ToString(), node.Value, nameC));
            }

            PreivewXmlFunction();
        }

        private void MenuOpenItem_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Language File (*.xml)|*.xml",
                Title = "Select Language File",
                Multiselect = false
            };

            if (ofd.ShowDialog() != true) return;
                var xml = XElement.Parse(File.ReadAllText(ofd.FileName));
                LoadXml(xml.Elements());
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
                File.WriteAllText(sfd.FileName, XmlToString());

                MessageBox.Show("Created language file", "Done");
            }
        }

        private string XmlToString()
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(sb, settings))
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

            return sb.ToString();
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

        private void PreivewXmlFunction()
        {
            var s = string.Empty;
            Task.Run(() =>
            {
                s = XmlToString();
            }).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    XMLPreview.Text = s;
                });
            });

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Got fucus?");

            var tbxCurrent = sender as TextBox;
            if (tbxCurrent == null) return;
            var tbkTitle = ((StackPanel)(tbxCurrent.Parent)).Children[0] as TextBlock;
            var mldCurrentValue = _listDatas.Where(x => tbkTitle != null && x.TitleCorrect.Equals(tbkTitle.Text)).ToList();

            if (mldCurrentValue.Count <= 0) return;
            var sNewTitle = mldCurrentValue[0].TitleCorrect + " : " + mldCurrentValue[0].Body;
            if (tbkTitle != null) tbkTitle.Text = sNewTitle;
            tbxCurrent.Text = string.Empty;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tbxCurrent = sender as TextBox;

            if (tbxCurrent != null && tbxCurrent.Text == string.Empty)
            {
                var tbkTitle = ((StackPanel)(tbxCurrent.Parent)).Children[0] as TextBlock;
                string[] sSpliters = { " : " };
                if (tbkTitle != null)
                {
                    var sTitleAndValue = tbkTitle.Text.Split(sSpliters, StringSplitOptions.RemoveEmptyEntries);
                    if (sTitleAndValue.Length > 1)
                    {
                        tbkTitle.Text = sTitleAndValue[0];
                        tbxCurrent.Text = sTitleAndValue[1];
                        _listDatas.Where(x => x.TitleCorrect.Equals(tbkTitle.Text)).ToList()[0].Body = sTitleAndValue[1];
                    }
                }
            }

            // Calling Toyz LostFocus function
            PreivewXmlFunction();
        }
    }
}
///////