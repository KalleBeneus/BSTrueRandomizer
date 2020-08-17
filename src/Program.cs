using System;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.modification;
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

            Random random = CreateSeededRandom(opts);
            var itemRandomizerService = new ItemRandomizerService(random);

            var randomizerService = new ItemPlacementRandomizerMod(itemRandomizerService);
            randomizerService.RandomizeItems(gameFiles);

            if (opts.IsRandomizeType)
            {
                var typeRandomizerService = new DropTypeRandomizerMod();
                typeRandomizerService.RandomizeTypesWithLimitedFixedKeyLocations(gameFiles.DropList);
            }

            GameFileService.WriteModifiedFiles(gameFiles, opts.OutputPath);
        }

        private static Random CreateSeededRandom(Options opts)
        {
            Random random;
            if (opts.SeedText == null)
            {
                random = new Random();
            }
            else
            {
                opts.Seed = SeedConverter.CalculateSeedNumberFromString(opts.SeedText);
                random = new Random(opts.Seed);
            }

            return random;
        }
    }
}