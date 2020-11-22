using System.IO;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.util;
using CommandLine;

namespace BSTrueRandomizer.config
{
    public class Options
    {
        private static readonly string DefaultDirectory = Directory.GetCurrentDirectory();

        [Option('s', "seed", Required = false,
            HelpText =
                "Randomization seed provided as a free text string. Not providing this will result in a random seed that is not retrievable by the user.")]
        public string? SeedText { get; set; }

        [Option('t', "type-randomize", Required = false, HelpText = "Randomize chest types. When playing on Retain Type, all non-consumable, non-crafting chests will be fully shuffled.")]
        public bool IsRandomizeType { get; set; }

        [Option('k', "key-locations", Default = 15, Required = false, HelpText = "Set the number of randomly selected key item locations. This controls the likelihood of key items appearing in chests guarded by bosses. Lower number means higher likelihood. Only valid together with -t flag.")]
        public int NumberOfKeyLocations { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output folder path. Defaults to current directory.")]
        public string OutputPath { get; set; } = DefaultDirectory;

        [Option('i', "input", Required = false,
            HelpText = "Input folder path where DropRate, Quest, Craft and Item master json/uasset files are located. Defaults to current directory.")]
        public string? InputPath { get; set; }

        [Option('u', "unrealpak-path", Required = false, HelpText = "Folder path where UnrealPak.exe is located. Defaults to current directory.")]
        public string UnrealPakPath { get; set; } = DefaultDirectory;

        [Option('j', "json-output", Default = false, Required = false, HelpText = "Output modified json files into output folder.")]
        public bool IsJsonOutput { get; set; }

        [Option('J', "json-only", Default = false, Required = false, HelpText = "Skip creation of .pak file. Only create and output modified json files.")]
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

            if (!Directory.Exists(UnrealPakPath))
            {
                throw new InputException($"Provided path to UnrealPak executable '{UnrealPakPath}' does not exist.");
            }

            bool isDefaultPakPath = UnrealPakPath.Equals(NormalizeFolderPath(DefaultDirectory));
            bool isUserProvidedPathEmpty = !isDefaultPakPath && !File.Exists(Path.Combine(UnrealPakPath, Constants.UnrealPakExeFileName));
            bool isDefaultPathEmpty = isDefaultPakPath && !File.Exists(Path.Combine(UnrealPakPath, Constants.UnrealPakExeFileName));
            bool isPakResourceProvided = File.Exists(Path.Combine(FileUtil.getResourcePath(), Constants.UnrealPakResourcePath));
            if (!IsJsonOnly && (isUserProvidedPathEmpty || (isDefaultPathEmpty && !isPakResourceProvided)))
            {
                throw new InputException(
                    $"Provided path to UnrealPak executable '{UnrealPakPath}' does not contain an executable named '{Constants.UnrealPakExeFileName}'.");
            }
        }

        public void NormalizeInput()
        {
            SeedText = SeedText?.Trim();
            OutputPath = NormalizeFolderPath(OutputPath)!;
            InputPath = NormalizeFolderPath(InputPath);
            UnrealPakPath = NormalizeFolderPath(UnrealPakPath)!;
        }

        private static string? NormalizeFolderPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
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