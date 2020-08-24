using System;
using System.IO;
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
            parserResult.WithParsed(RunMain);
        }

        public static void RunMain(Options opts)
        {
            var gameFileReader = new GameFileService(opts.InputPath);
            GameFiles gameFiles = gameFileReader.ReadAllFiles();

            Random random = CreateSeededRandom(opts.SeedText);
            var itemRandomizerService = new ItemRandomizerService(random);
            var randomizerService = new ItemPlacementRandomizerMod(itemRandomizerService);
            var typeRandomizerService = new DropTypeRandomizerMod(itemRandomizerService);


            randomizerService.RandomizeItems(gameFiles);
            if (opts.IsRandomizeType)
            {
                typeRandomizerService.RandomizeTypesWithLimitedFixedKeyLocations(gameFiles.DropList);
            }

            if (opts.IsJsonOutput || opts.IsJsonOnly)
            {
                GameFileService.WriteModifiedJsonFiles(gameFiles, opts.OutputPath);
            }

            if (!opts.IsJsonOnly)
            {
                string assetOutputFolder = Path.Combine(opts.OutputPath, Constants.uassetPathBase, Constants.uassetPathSub);
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