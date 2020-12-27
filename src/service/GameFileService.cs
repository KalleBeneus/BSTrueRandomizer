using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BSTrueRandomizer.config;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.model.values;
using BSTrueRandomizer.util;
using Newtonsoft.Json;

namespace BSTrueRandomizer.service
{
    public class GameFileService
    {
        private readonly Options _options;
        private readonly ItemTypeConverter _itemTypeConverter;
        private readonly UassetService _uassetService;

        public GameFileService(Options options, UassetService uassetService)
        {
            _options = options;
            _uassetService = uassetService;

            string jsonFileName = FileUtil.GetJsonFileName(Constants.FileNameItemMaster);
            string itemMasterFileContents = FileUtil.ReadFileAsText(options.InputPath, jsonFileName);
            var masterItemList = JsonConvert.DeserializeObject<List<MasterItem>>(itemMasterFileContents);
            _itemTypeConverter = new ItemTypeConverter(masterItemList);
        }

        public GameFiles ReadAllFiles(string folderPath = "")
        {
            string? path = string.IsNullOrWhiteSpace(folderPath) ? _options.InputPath : folderPath;

            string jsonFileName = FileUtil.GetJsonFileName(Constants.FileNameDropRateMaster);
            string dropMasterString = FileUtil.ReadFileAsText(path, jsonFileName);
            var dropList = JsonConvert.DeserializeObject<List<DropItemEntry>>(dropMasterString);

            jsonFileName = FileUtil.GetJsonFileName(Constants.FileNameQuestMaster);
            string questMasterString = FileUtil.ReadFileAsText(path, jsonFileName);
            var questList = JsonConvert.DeserializeObject<List<QuestItemEntry>>(questMasterString);

            jsonFileName = FileUtil.GetJsonFileName(Constants.FileNameCraftMaster);
            string craftMasterString = FileUtil.ReadFileAsText(path, jsonFileName);
            var craftList = JsonConvert.DeserializeObject<List<CraftItemEntry>>(craftMasterString);

            SetItemTypesByNameLookup(questList);
            SetItemTypesByNameLookup(dropList);
            SetItemTypesByNameLookup(craftList);

            return new GameFiles(craftList, dropList, questList);
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

        public void WritePackagedModFile(GameFiles gameFiles, FilePath packageOutputFilePath)
        {
            string tempFolder = Path.Combine(packageOutputFilePath.DirectoryPath, Constants.TempFolderName);
            string tempAssetsFullPath = Path.Combine(tempFolder, Constants.UassetBaseFolderName, Constants.UassetSubPath);
            bool isTmpDirAlreadyExists = Directory.Exists(tempFolder);
            Directory.CreateDirectory(tempAssetsFullPath);

            try
            {
                WriteModifiedUassetFile(gameFiles.DropList, new FilePath(tempAssetsFullPath, Constants.FileNameDropRateMaster));
                WriteModifiedUassetFile(gameFiles.QuestList, new FilePath(tempAssetsFullPath, Constants.FileNameQuestMaster));
                WriteModifiedUassetFile(gameFiles.CraftList, new FilePath(tempAssetsFullPath, Constants.FileNameCraftMaster));
                CreatePakFile(packageOutputFilePath, tempFolder);
            }
            finally
            {
                if (!isTmpDirAlreadyExists)
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        private void WriteModifiedUassetFile(IEnumerable<IItemEntry> gameFile, FilePath outputFilePath)
        {
            string modifiedJson = JsonConvert.SerializeObject(gameFile);

            string uassetFileName = FileUtil.GetUassetFileName(outputFilePath.FileName);
            byte[] uassetData = FileUtil.ReadFileAsBytes(_options.InputPath, uassetFileName);

            _uassetService.WriteModifiedUassetFile(modifiedJson, uassetData, outputFilePath);
        }

        public void CreatePakFile(FilePath outputFileInfo, string pakContentsParentPath)
        {
            string fileListPath = WritePackageDescriptionFileToDisk(pakContentsParentPath);

            using var pProcess = new Process
            {
                StartInfo =
                {
                    FileName = GetUnrealPakPath(),
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

        private string GetUnrealPakPath()
        {
            string userProvidedUnrealPakPath = Path.Combine(_options.UnrealPakPath, Constants.UnrealPakExeFileName);
            if (!File.Exists(userProvidedUnrealPakPath) || File.Exists(Constants.UnrealPakResourcePath))
            {
                return FileUtil.getResourcePath(Constants.UnrealPakResourcePath);
            }

            return userProvidedUnrealPakPath;
        }

        private static string WritePackageDescriptionFileToDisk(string pakContentsParentFolder)
        {
            string fileListPath = Path.Combine(pakContentsParentFolder, Constants.PackageDescriptionFileName);
            string uassetBasePath = Path.Combine(pakContentsParentFolder, Constants.UassetBaseFolderName);
            string fileListContent = $"\"{uassetBasePath}\\*.*\" \"..\\..\\..\\*.*\"";
            File.WriteAllText(fileListPath, fileListContent);
            return fileListPath;
        }
    }
}