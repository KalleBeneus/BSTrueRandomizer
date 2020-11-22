using System;
using System.IO;
using System.Resources;
using System.Text;
using BSTrueRandomizer.config;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.model.values;
using BSTrueRandomizer.Properties;

namespace BSTrueRandomizer.util
{
    public class FileUtil
    {
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

        public static string ReadFileAsText(string? userProvidedPath, string fileName)
        {
            byte[] gameFileAsBytes = ReadFileAsBytes(userProvidedPath, fileName);
            if (gameFileAsBytes.Length == 0)
            {
                throw new InputException($"'{fileName}' could not be found. You can specify the folder where the file is located with --input <folder path>");
            }
            return ConvertBytesToString(gameFileAsBytes);

        }

        private static string ConvertBytesToString(byte[] gameFileAsBytes)
        {
            using var stream = new MemoryStream(gameFileAsBytes);
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static byte[] ReadFileAsBytes(string? userProvidedPath, string fileName)
        {
            
            if (!string.IsNullOrWhiteSpace(userProvidedPath))
            {
                var providedFilePath = new FilePath(userProvidedPath, fileName);
                return TryReadFileContents(providedFilePath);
            }

            var defaultFilePath = new FilePath(Directory.GetCurrentDirectory(), fileName);
            return File.Exists(defaultFilePath.FullPath) ? File.ReadAllBytes(defaultFilePath.FullPath) : GetResourceFileAsByteArray(fileName);
        }

        private static byte[] TryReadFileContents(FilePath filePath)
        {
            if (!File.Exists(filePath.FullPath))
            {
                throw new InputException(
                    $"'{filePath.FileName}' file could not be found in provided input folder '{filePath.DirectoryPath}'. Add the file or specify another folder with --input <folder path>");
            }

            return File.ReadAllBytes(filePath.FullPath);
        }

        public static byte[] GetResourceFileAsByteArray(string fileName)
        {
            try
            {
                if (!(Resources.ResourceManager.GetObject(fileName) is byte[] bytes))
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
    }
}