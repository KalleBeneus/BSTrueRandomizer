using CommandLine;

namespace BSTrueRandomizer
{
    public class Options
    {
        [Option('s', "seed", Required = false, HelpText = "Randomization seed provided as a free text string")]
        public string SeedText { get; set; } = null;

        [Option('t', "type-randomize", Required = false, HelpText = "Randomize chest types")]
        public bool IsRandomizeType { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output folder path")]
        public string OutputPath { get; set; } = "./";

        [Option('i', "input", Required = false, HelpText = "Input folder path where DropRate, Quest, Craft and Item master files are located")]
        public string InputPath { get; set; } = Constants.DefaultInputFolderPath;

        public int Seed { get; set; }
    }
}