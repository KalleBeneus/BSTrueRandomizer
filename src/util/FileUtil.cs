using System;
using System.IO;
using System.Resources;
using BSTrueRandomizer.Exceptions;

namespace BSTrueRandomizer.util
{
    public class FileUtil
    {
        public static string AddFolderSeparatorIfMissing(string folderPath)
        {
            return Path.TrimEndingDirectorySeparator(folderPath) + Path.DirectorySeparatorChar;
        }

        public static string GetResourceFileAsString(string fileName)
        {
            try
            {
                object fileObject = Properties.Resources.ResourceManager.GetObject(fileName);
                return fileObject != null ? System.Text.Encoding.UTF8.GetString(fileObject as byte[]) : "";
            }
            catch (Exception ex) when (ex is MissingManifestResourceException || ex is MissingSatelliteAssemblyException)
            {
                throw new InputException($"'{fileName}' could not be found. You can specify the folder where the file is located with -input <folder path>");
            }
        }

        public static byte[] GetResourceFileAsByteArray(string fileName)
        {
            try
            {
                if (!(Properties.Resources.ResourceManager.GetObject(fileName) is byte[] bytes))
                {
                    return new byte[0];
                }
                return bytes;
            }
            catch (Exception ex) when (ex is MissingManifestResourceException || ex is MissingSatelliteAssemblyException)
            {
                return new byte[0];
            }
        }

        public static string GetJsonFilePath(string path, string fileName)
        {
            return Path.Combine(path, fileName, Constants.FileExtensionJson);
        }

        public static string GetJsonFileName(string fileName)
        {
            return Path.ChangeExtension(fileName, Constants.FileExtensionJson);
        }

        public static string GetUassetFileName(string fileName)
        {
            return Path.ChangeExtension(fileName, Constants.FileExtensionUasset);
        }

        public static string GetBinFileName(string fileName)
        {
            return Path.ChangeExtension(fileName, Constants.FileExtensionBin);
        }
    }
}
