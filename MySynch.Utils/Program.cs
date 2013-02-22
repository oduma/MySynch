using System;
using System.IO;
using System.Xml.Serialization;
using CommandLine;
using CommandLine.Text;
using MySynch.Core;
using MySynch.Core.DataTypes;

namespace MySynch.Utils
{
    class Program
    {
        internal static readonly HeadingInfo headingInfo = new HeadingInfo(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

        static void Main(string[] args)
        {


            var options = new Options();

            ICommandLineParser parser= new CommandLineParser();
            if (parser.ParseArguments(args, options,Console.Error))
            {
                if (!string.IsNullOrEmpty(options.StartFromFolder))
                {
                    BuildFromFolder(options.StartFromFolder, options.OutputFile);
                }
            }
        }

        private static void BuildFromFolder(string startFromFolder, string outputFile)
        {
            var itemDiscoverer = new ItemDiscoverer(startFromFolder);

            itemDiscoverer.DiscoveringFolder += new EventHandler<FolderDiscoveredArg>(itemDiscoverer_DiscoveringFolder);
            var synchItem = itemDiscoverer.DiscoverFromFolder(startFromFolder);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SynchItem));

            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                xmlSerializer.Serialize(fs, synchItem);

        }

        static void itemDiscoverer_DiscoveringFolder(object sender, FolderDiscoveredArg e)
        {
            Console.WriteLine(e.Folder);
        }
    }
}
