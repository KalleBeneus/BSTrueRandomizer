using System.Globalization;
using System.IO;
using System.Threading;
using BSTrueRandomizer.model.values;
using BSTrueRandomizer.util;
using UAssetParser;
using UAssetParser.Objects.Visitors;

namespace BSTrueRandomizer.service
{
    public class UassetService
    {

        public UassetService()
        {
            InitializeUassetWriteMode();
        }

        private void InitializeUassetWriteMode()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            VisitorFactory.LoadVisitors();
            UAsset.Options = new UAParserOptions(forceArrays: true, newEntries: true);
        }

        public void WriteModifiedUassetFile(string modifiedJson, byte[] uassetData, FilePath outputFileInfo)
        {
            string outputFolder = outputFileInfo.DirectoryPath;
            string outputFileName = outputFileInfo.FileName;
            var uAsset = new UAsset(new MemoryStream(uassetData));

            uAsset.UpdateFromJSON(modifiedJson);
            uAsset.SerializeToBinary(outputFileInfo.FullPath);

            string uassetFilePath = Path.Combine(outputFolder, FileUtil.GetUassetFileName(outputFileName));
            string binFilePath = Path.Combine(outputFolder, FileUtil.GetBinFileName(outputFileName));
            File.Move(binFilePath, uassetFilePath, true);
        }
    }
}