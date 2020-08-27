using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.util;
using Newtonsoft.Json;
using UAssetParser;
using UAssetParser.Objects.Visitors;

namespace BSTrueRandomizer.service
{
    public class GameFileService
    {
        private readonly string _inputFolderPath;
        private readonly ItemTypeConverter _itemTypeConverter;

        public GameFileService(string inputFolder)
        {
            _inputFolderPath = inputFolder;
            string fileText = ReadGameFileText(_inputFolderPath, Constants.FileNameItemMaster);
            var masterItemList = JsonConvert.DeserializeObject<List<MasterItem>>(fileText);
            _itemTypeConverter = new ItemTypeConverter(masterItemList);
        }

        public GameFiles ReadAllFiles(string folderPath = "")
        {
            string path = string.IsNullOrWhiteSpace(folderPath) ? _inputFolderPath : FileUtil.AddFolderSeparatorIfMissing(folderPath);

            string dropMasterString = ReadGameFileText(path, Constants.FileNameDropRateMaster);
            var dropList = JsonConvert.DeserializeObject<List<DropItemEntry>>(dropMasterString);
            string questMasterString = ReadGameFileText(path, Constants.FileNameQuestMaster);
            var questList = JsonConvert.DeserializeObject<List<QuestItemEntry>>(questMasterString);
            string craftMasterString = ReadGameFileText(path, Constants.FileNameCraftMaster);
            var craftList = JsonConvert.DeserializeObject<List<CraftItemEntry>>(craftMasterString);

            SetItemTypesByNameLookup(questList);
            SetItemTypesByNameLookup(dropList);
            SetItemTypesByNameLookup(craftList);

            return new GameFiles(craftList, dropList, questList);
        }

        private string ReadGameFileText(string userProvidedPath, string fileName)
        {
            string jsonFileName = FileUtil.GetJsonFileName(fileName);
            string defaultFilePath = Path.Combine(Constants.DefaultInputFolderPath, jsonFileName);
            if (!string.IsNullOrWhiteSpace(userProvidedPath))
            {
                return TryReadFileContents(userProvidedPath, jsonFileName);
            }

            return File.Exists(defaultFilePath) ? File.ReadAllText(defaultFilePath) : FileUtil.GetResourceFileAsString(jsonFileName);
        }

        private string TryReadFileContents(string path, string jsonFileName)
        {
            string filePath = Path.Combine(path, jsonFileName);
            if (!File.Exists(filePath))
            {
                throw new InputException(
                    $"'{jsonFileName}' file could not be found in input folder '{path}'. Add the file or specify another folder with -input <folder path>");
            }

            return File.ReadAllText(filePath);
        }

        private void SetItemTypesByNameLookup(IEnumerable<IItemEntry> questList)
        {
            foreach (IItemEntry entry in questList)
            {
                entry.UpdateItemType(_itemTypeConverter.FindAndConvertItemTypeFromName(entry.GetItemName()));
            }
        }

        public static void WriteModifiedJsonFiles(GameFiles gameFiles, string outputFolder)
        {
            string outputFolderPath = FileUtil.AddFolderSeparatorIfMissing(outputFolder);

            WriteModifiedJsonFile(gameFiles.DropList, Constants.FileNameDropRateMaster, outputFolderPath);
            WriteModifiedJsonFile(gameFiles.QuestList, Constants.FileNameQuestMaster, outputFolderPath);
            WriteModifiedJsonFile(gameFiles.CraftList, Constants.FileNameCraftMaster, outputFolderPath);
        }

        private static void WriteModifiedJsonFile(IEnumerable<IItemEntry> modifiedData, string fileName, string outputFolderPath)
        {
            string jsonOutput = JsonConvert.SerializeObject(modifiedData);
            File.WriteAllText(Path.Combine(outputFolderPath, FileUtil.GetJsonFileName(fileName)), jsonOutput, Encoding.UTF8);
        }

        public static void WriteModifiedUassetFiles(GameFiles gameFiles, string outputFolder)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            VisitorFactory.LoadVisitors();
            UAsset.Options = new UAParserOptions(forceArrays: true, newEntries: true);

            WriteModifiedUassetFile(gameFiles.DropList, Constants.FileNameDropRateMaster, outputFolder);
            WriteModifiedUassetFile(gameFiles.QuestList, Constants.FileNameQuestMaster, outputFolder);
            WriteModifiedUassetFile(gameFiles.CraftList, Constants.FileNameCraftMaster, outputFolder);
        }

        private static void WriteModifiedUassetFile(IEnumerable<IItemEntry> gameFile, string fileName, string outputFolder)
        {
            byte[] uassetData = FileUtil.GetResourceFileAsByteArray(FileUtil.GetUassetFileName(fileName));
            var asset = new UAsset(new MemoryStream(uassetData));
            string modifiedJson = JsonConvert.SerializeObject(gameFile);
            Directory.CreateDirectory(outputFolder);

            asset.UpdateFromJSON(modifiedJson);
            asset.SerializeToBinary(Path.Combine(outputFolder, fileName));

            string uassetFilePath = Path.Combine(outputFolder, FileUtil.GetUassetFileName(fileName));
            string binFilePath = Path.Combine(outputFolder, FileUtil.GetBinFileName(fileName));
            File.Move(binFilePath, uassetFilePath, true);
        }

        public static void CreatePakFile(Options opts)
        {
            string fileListPath = Path.Combine(opts.OutputPath, "filelist.txt");
            string uassetBasePath = Path.Combine(opts.OutputPath, Constants.UassetPathBase);
            File.WriteAllText(fileListPath, $"\"{uassetBasePath}*.*\" \"..\\..\\..\\*.*\"");

            string pakFileName = string.IsNullOrWhiteSpace(opts.SeedText) ? Constants.DefaultPakFileName : opts.SeedText;
            pakFileName = Path.ChangeExtension(pakFileName, Constants.FileExtensionPak);
            string pakFilePath = Path.Combine(opts.OutputPath, pakFileName);

            using var pProcess = new Process();

            pProcess.StartInfo.FileName = @"resources\UnrealPak.exe";
            pProcess.StartInfo.Arguments = $"\"{pakFilePath}\" -create=\"{fileListPath}\"";
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            Console.WriteLine(pProcess.StandardOutput.ReadToEnd());
            pProcess.WaitForExit();

            Directory.Delete(uassetBasePath, true);
            File.Delete(fileListPath);
        }
    }
}