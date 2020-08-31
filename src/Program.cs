using System;
using System.IO;
using BSTrueRandomizer.config;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.mod;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.service;
using BSTrueRandomizer.util;
using CommandLine;

namespace BSTrueRandomizer

    // TODO Error handling with friendly error messages on console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);
            try
            {
                parserResult.WithParsed(o => o.NormalizeInput())
                    .WithParsed(o => o.Validate())
                    .WithParsed(RunMain);
            }
            catch (RandomizerBaseException e)
            {
                Console.WriteLine($@"ERROR: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(@"ERROR: An unexpected problem occurred. If the problem persists, contact the project maintainer.");
                Console.WriteLine($@"Details: {e.Message}");
            }
        }

        public static void RunMain(Options opts)
        {
            var gameFileReader = new GameFileService(opts.InputPath);
            GameFiles gameFiles = gameFileReader.ReadAllFiles();

            Random random = CreateSeededRandom(opts.SeedText);
            var itemRandomizerService = new ItemRandomizerService(random);
            var itemPlacementRandomizerMod = new ItemPlacementRandomizerMod(itemRandomizerService);
            var dropTypeRandomizerMod = new DropTypeRandomizerMod(itemRandomizerService, opts);

            itemPlacementRandomizerMod.RandomizeItems(gameFiles);

            if (opts.IsRandomizeType)
            {
                dropTypeRandomizerMod.SetAllItemLocationsToSameType(gameFiles.DropList);
                dropTypeRandomizerMod.SetRandomKeyItemLocations(gameFiles.DropList);
            }

            if (opts.IsJsonOutput || opts.IsJsonOnly)
            {
                GameFileService.WriteModifiedJsonFiles(gameFiles, opts.OutputPath);
            }

            if (!opts.IsJsonOnly)
            {
                string assetOutputFolder = Path.Combine(opts.OutputPath, Constants.UassetPathRelativeBase, Constants.UassetPathRelativeSub);
                GameFileService.WriteModifiedUassetFiles(gameFiles, assetOutputFolder);
                GameFileService.CreatePakFile(opts);
            }
        }

        private static Random CreateSeededRandom(string seedText)
        {
            return string.IsNullOrWhiteSpace(seedText) ? new Random() : new Random(SeedConverter.CalculateSeedNumberFromString(seedText));
        }
    }
}