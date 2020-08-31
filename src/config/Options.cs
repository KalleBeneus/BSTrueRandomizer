using System.IO;
using BSTrueRandomizer.Exceptions;
using CommandLine;

namespace BSTrueRandomizer.config
{
    public class Options
    {
        [Option('s', "seed", Required = false,
            HelpText =
                "Randomization seed provided as a free text string. Not providing this will result in a random seed that is not retrievable by the user.")]
        public string SeedText { get; set; }

        [Option('t', "type-randomize", Required = false, HelpText = "Randomize chest types")]
        public bool IsRandomizeType { get; set; }

        [Option('k', "key-locations", Required = false, HelpText = "Set the number of randomly selected key item locations. Defaults to 15.")]
        public int NumberOfKeyLocations { get; set; } = 15;

        [Option('o', "output", Required = false, HelpText = "Output folder path")]
        public string OutputPath { get; set; } = Directory.GetCurrentDirectory();

        [Option('i', "input", Required = false, HelpText = "Input folder path where DropRate, Quest, Craft and Item master files are located")]
        public string InputPath { get; set; }

        [Option('j', "json-output", Required = false, HelpText = "Output modified json files into output folder.")]
        public bool IsJsonOutput { get; set; }

        [Option('J', "json-only", Required = false, HelpText = "Skip creation of .pak file. Only create and output modified json files.")]
        public bool IsJsonOnly { get; set; }


        public void Validate()
        {
            if (SeedText != null && string.IsNullOrWhiteSpace(SeedText))
            {
                throw new InputException("Provided seed value was empty. If this was intended, run the program without the seed option to get a random seed.");
            }

            if (NumberOfKeyLocations < 0)
            {
                throw new InputException("Provided number of key locations cannot be negative.");
            }

            if (!Directory.Exists(OutputPath))
            {
                throw new InputException($"Provided output folder '{OutputPath}' does not exist.");
            }

            if (InputPath != null && !Directory.Exists(InputPath))
            {
                throw new InputException($"Provided input folder '{InputPath}' does not exist.");
            }
        }

        public void NormalizeInput()
        {
            SeedText = SeedText?.Trim();
            OutputPath = NormalizeFolderPath(OutputPath);
            InputPath = NormalizeFolderPath(InputPath);
        }

        private static string NormalizeFolderPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            string normalizedPath = path.Trim();
            normalizedPath = normalizedPath.Trim('"');
            if (!Path.EndsInDirectorySeparator(normalizedPath))
            {
                normalizedPath += Path.DirectorySeparatorChar;
            }

            return normalizedPath;
        }
    }
}