using System.Collections.Generic;
using System.IO;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.util;
using Newtonsoft.Json;

namespace BSTrueRandomizer.service
{
    public class GameFileService
    {
        private readonly string _inputFolderPath;
        private readonly ItemTypeConverter _itemTypeConverter;

        public GameFileService(string inputFolder)
        {
            _inputFolderPath = FileUtil.AddFolderSeparatorIfMissing(inputFolder);
            _itemTypeConverter = new ItemTypeConverter(inputFolder);
        }

        public GameFiles ReadAllFiles(string folderPath = "")
        {
            string path = string.IsNullOrWhiteSpace(folderPath) ? _inputFolderPath : FileUtil.AddFolderSeparatorIfMissing(folderPath);

            string dropMasterString = File.ReadAllText(path + Constants.FileNameDropRateMaster);
            var dropList = JsonConvert.DeserializeObject<List<DropItemEntry>>(dropMasterString);
            string questMasterString = File.ReadAllText(path + Constants.FileNameQuestMaster);
            var questList = JsonConvert.DeserializeObject<List<QuestItemEntry>>(questMasterString);
            string craftMasterString = File.ReadAllText(path + Constants.FileNameCraftMaster);
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

        public static void WriteModifiedFiles(GameFiles gameFiles, string outputFolder)
        {
            string outputFolderPath = FileUtil.AddFolderSeparatorIfMissing(outputFolder);

            string jsonOutput = JsonConvert.SerializeObject(gameFiles.DropList);
            File.WriteAllText(outputFolderPath + Constants.FileNameDropRateMaster, jsonOutput);
            jsonOutput = JsonConvert.SerializeObject(gameFiles.QuestList);
            File.WriteAllText(outputFolderPath + Constants.FileNameQuestMaster, jsonOutput);
            jsonOutput = JsonConvert.SerializeObject(gameFiles.CraftList);
            File.WriteAllText(outputFolderPath + Constants.FileNameCraftMaster, jsonOutput);
        }
    }
}