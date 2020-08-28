using CommandLine;

namespace BSTrueRandomizer
{
    public class Options
    {
        [Option('s', "seed", Required = false, HelpText = "Randomization seed provided as a free text string")]
        public string SeedText { get; set; } = null;

        [Option('t', "type-randomize", Required = false, HelpText = "Randomize chest types")]
        public bool IsRandomizeType { get; set; }

        [Option('k', "key-locations", Required = false, HelpText = "Set the number of randomly selected key item locations. Defaults to 15.")]
        public int NumberOfKeyLocations { get; set; } = 15;

        [Option('o', "output", Required = false, HelpText = "Output folder path")]
        public string OutputPath { get; set; } = "./";

        [Option('i', "input", Required = false, HelpText = "Input folder path where DropRate, Quest, Craft and Item master files are located")]
        public string InputPath { get; set; }

        [Option('j', "json-output", Required = false, HelpText = "Output modified json files into output folder.")]
        public bool IsJsonOutput { get; set; }

        [Option('J', "json-only", Required = false, HelpText = "Skip creation of .pak file. Only create and output modified json files.")]
        public bool IsJsonOnly { get; set; }
    }
}