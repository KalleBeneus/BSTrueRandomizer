using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BSTrueRandomizer.config;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.model.values;
using BSTrueRandomizer.util;
using Newtonsoft.Json;

namespace BSTrueRandomizer.service
{
    public class GameFileService
    {
        private readonly string? _inputFolderPath;
        private readonly ItemTypeConverter _itemTypeConverter;
        private readonly UassetService _uassetService;

        public GameFileService(string? inputFolder, UassetService uassetService)
        {
            _inputFolderPath = inputFolder;
            _uassetService = uassetService;

            string fileText = ReadGameFileText(_inputFolderPath, Constants.FileNameItemMaster);
            var masterItemList = JsonConvert.DeserializeObject<List<MasterItem>>(fileText);
            _itemTypeConverter = new ItemTypeConverter(masterItemList);
        }

        public GameFiles ReadAllFiles(string folderPath = "")
        {
            string? path = string.IsNullOrWhiteSpace(folderPath) ? _inputFolderPath : folderPath;

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

        private string ReadGameFileText(string? userProvidedPath, string fileName)
        {
            string jsonFileName = FileUtil.GetJsonFileName(fileName);
            if (!string.IsNullOrWhiteSpace(userProvidedPath))
            {
                var providedFilePath = new FilePath(userProvidedPath, jsonFileName);
                return TryReadFileContents(providedFilePath);
            }

            var defaultFilePath = new FilePath(Directory.GetCurrentDirectory(), jsonFileName);
            return File.Exists(defaultFilePath.FullPath) ? File.ReadAllText(defaultFilePath.FullPath) : FileUtil.GetResourceFileAsString(jsonFileName);
        }

        private static string TryReadFileContents(FilePath filePath)
        {
            if (!File.Exists(filePath.FullPath))
            {
                throw new InputException(
                    $"'{filePath.FileName}' file could not be found in input folder '{filePath.DirectoryPath}'. Add the file or specify another folder with --input <folder path>");
            }

            return File.ReadAllText(filePath.FullPath);
        }

        private void SetItemTypesByNameLookup(IEnumerable<IItemEntry> questList)
        {
            foreach (IItemEntry entry in questList)
            {
                entry.UpdateItemType(_itemTypeConverter.FindAndConvertItemTypeFromName(entry.GetItemName()));
            }
        }

        public void WriteModifiedJsonFiles(GameFiles gameFiles, string outputFolder)
        {
            WriteModifiedJsonFile(gameFiles.DropList, new FilePath(outputFolder, Constants.FileNameDropRateMaster, Constants.FileExtensionJson));
            WriteModifiedJsonFile(gameFiles.QuestList, new FilePath(outputFolder, Constants.FileNameQuestMaster, Constants.FileExtensionJson));
            WriteModifiedJsonFile(gameFiles.CraftList, new FilePath(outputFolder, Constants.FileNameCraftMaster, Constants.FileExtensionJson));
        }

        private static void WriteModifiedJsonFile(IEnumerable<IItemEntry> modifiedData, FilePath outputFilePath)
        {
            string jsonOutput = JsonConvert.SerializeObject(modifiedData);
            File.WriteAllText(outputFilePath.FullPath, jsonOutput, Encoding.UTF8);
        }

        public void WritePackagedModFile(GameFiles gameFiles, FilePath packageFileInfo)
        {
            string tempFolder = Path.Combine(packageFileInfo.DirectoryPath, Constants.TempFolderName);
            string tempAssetsFullPath = Path.Combine(tempFolder, Constants.UassetBaseFolderName, Constants.UassetSubPath);
            Directory.CreateDirectory(tempAssetsFullPath);

            WriteModifiedUassetFile(gameFiles.DropList, new FilePath(tempAssetsFullPath, Constants.FileNameDropRateMaster));
            WriteModifiedUassetFile(gameFiles.QuestList, new FilePath(tempAssetsFullPath, Constants.FileNameQuestMaster));
            WriteModifiedUassetFile(gameFiles.CraftList, new FilePath(tempAssetsFullPath, Constants.FileNameCraftMaster));

            CreatePakFile(packageFileInfo, tempFolder);

            Directory.Delete(tempFolder, true);
        }

        private void WriteModifiedUassetFile(IEnumerable<IItemEntry> gameFile, FilePath outputFilePath)
        {
            string modifiedJson = JsonConvert.SerializeObject(gameFile);
            byte[] uassetData = FileUtil.GetResourceFileAsByteArray(FileUtil.GetUassetFileName(outputFilePath.FileName));
            _uassetService.WriteModifiedUassetFile(modifiedJson, uassetData, outputFilePath);
        }

        public static void CreatePakFile(FilePath outputFileInfo, string pakContentsParentPath)
        {
            string fileListPath = WriteFileList(pakContentsParentPath);
            using var pProcess = new Process
            {
                StartInfo =
                {
                    FileName = @"resources\UnrealPak.exe",
                    Arguments = $"\"{outputFileInfo.FullPath}\" -create=\"{fileListPath}\"",
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };

            pProcess.Start();
            Console.WriteLine(pProcess.StandardOutput.ReadToEnd());
            pProcess.WaitForExit();
        }

        private static string WriteFileList(string pakContentsParentFolder)
        {
            string fileListPath = Path.Combine(pakContentsParentFolder, "filelist.txt");
            string uassetBasePath = Path.Combine(pakContentsParentFolder, Constants.UassetBaseFolderName);
            string fileListContent = $"\"{uassetBasePath}*.*\" \"..\\..\\..\\*.*\"";
            File.WriteAllText(fileListPath, fileListContent);
            return fileListPath;
        }

    }
}