using CommandLine;

namespace PartnerCenter.Console.Commands
{
    [Verb("update", HelpText = "Update UWP application")]
    public class UpdateCommand
    {
        [Option('f', "flight", Required = true, HelpText = "The name of the package flight to upload the bundle to.")]
        public string Flight { get; set; } = null!;

        [Option('b', "bundle", Required = true, HelpText = "The path to *.msixupload bundle")]
        public string BundlePath { get; set; } = null!;
    }
}