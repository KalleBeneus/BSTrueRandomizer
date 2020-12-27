using System;
using System.IO;
using BSTrueRandomizer.config;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.mod;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.model.values;
using BSTrueRandomizer.service;
using BSTrueRandomizer.util;
using CommandLine;

namespace BSTrueRandomizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);
            try
            {
                parserResult.WithParsed(options => options.NormalizeInput())
                    .WithParsed(options => options.Validate())
                    .WithParsed(RunMain);
            }
            catch (RandomizerBaseException e)
            {
                Console.WriteLine($@"ERROR: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: An unexpected problem occurred. If the problem persists, contact the project maintainer.");
                Console.WriteLine($"\tDetails: {e.Message}; Location: {e.TargetSite?.Name}");
            }
        }

        public static void RunMain(Options opts)
        {
            Random random = CreateSeededRandom(opts.SeedText);
            var itemRandomizerService = new ItemRandomizerService(random);
            var uassetService = new UassetService();

            var gameFileService = new GameFileService(opts, uassetService);

            var itemPlacementRandomizerMod = new ItemPlacementRandomizerMod(itemRandomizerService);
            var dropTypeRandomizerMod = new DropTypeRandomizerMod(itemRandomizerService, opts);
            var modManager = new ModManager(opts, itemPlacementRandomizerMod, dropTypeRandomizerMod);

            CreateTrueRandomizerMod(opts, gameFileService, modManager);
        }

        private static Random CreateSeededRandom(string? seedText)
        {
            return string.IsNullOrWhiteSpace(seedText) ? new Random() : new Random(SeedConverter.CalculateSeedNumberFromString(seedText));
        }

        private static void CreateTrueRandomizerMod(Options opts, GameFileService gameFileService, ModManager modManager)
        {
            GameFiles gameFiles = gameFileService.ReadAllFiles();

            modManager.ApplyMods(gameFiles);

            if (opts.IsJsonOutput || opts.IsJsonOnly)
            {
                gameFileService.WriteModifiedJsonFiles(gameFiles, opts.OutputPath);
            }

            if (!opts.IsJsonOnly)
            {
                var packageFilePath = new FilePath(Path.GetFullPath(opts.OutputPath), opts.SeedText, Constants.FileExtensionPak, Constants.DefaultPakFileName);
                gameFileService.WritePackagedModFile(gameFiles, packageFilePath);
            }
        }
    }
}