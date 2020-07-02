using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using ControlClient;

namespace EzControlGUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var xmlPath = AppDomain.CurrentDomain.BaseDirectory + "ControllerConfig.xml";

            var xd = XDocument.Load(xmlPath);
            var s = new XmlSerializer(typeof(Processes));
            var processes = (Processes) s.Deserialize(xd.CreateReader());

            var client = new Client(-999);

            if (!client.IsListening())
            {
                System.Diagnostics.Process.Start(processes.ServerURI);
            }

            InitializeComponent();


            foreach (var tog in processes.Process.Select(p => new UcProcessButton(p.Name, p.URI, p.ProcessID)))
                SpMain.Children.Add(tog);
        }
    }

    [XmlRoot(ElementName = "Process")]
    public class Process
    {
        [XmlElement(ElementName = "ProcessID")] public int ProcessID { get; set; }

        [XmlElement(ElementName = "Name")] public string Name { get; set; }

        [XmlElement(ElementName = "URI")] public string URI { get; set; }
    }

    [XmlRoot(ElementName = "Processes")]
    public class Processes
    {
        [XmlElement(ElementName = "ServerURI")]
        public string ServerURI { get; set; }

        [XmlElement(ElementName = "Process")] public List<Process> Process { get; set; }
    }
}