using CommandLine;
using CommandLine.Text;

namespace MySynch.Utils
{
    public class Options
    {

        [Option("t", "test", DefaultValue = false, HelpText = "Generates a xml file based on data from memory.", MutuallyExclusiveSet = "action")]
        public bool BuildTestXml { get; set; }

        [Option("i", "inFolder", DefaultValue = "", HelpText = "Generates a xml file based on data from the folder structure.", MutuallyExclusiveSet = "action")]
        public string StartFromFolder { get; set; }

        [Option("o", "outputFile", DefaultValue = "items.xml", HelpText = "The name of the xml file  to be generated.", Required = true)]
        public string OutputFile { get; set; }

        [HelpOption("?", null, HelpText = "Display this help on the screen.")]
        public string GetUssage()
        {
            HelpText help = new HelpText(Program.headingInfo);
            help.AddOptions(this);
            return help.ToString();
        }
    }
}
