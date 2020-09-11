using System;
using System.IO;

namespace BSTrueRandomizer.model.values
{
    public class FilePath
    {
        public string DirectoryPath { get; }
        public string FileName { get; }
        public string FullPath { get; }

        public FilePath(string directoryPath, string? fileName, string extension) : this(directoryPath, fileName, extension, "")
        {
        }

        public FilePath(string directoryPath, string? fileName, string extension = "", string defaultFileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileName) && string.IsNullOrWhiteSpace(defaultFileName))
            {
                throw new ArgumentException("A non-empty filename must be provided when creating FilePath instance");
            }

            fileName = string.IsNullOrWhiteSpace(extension) ? fileName : Path.ChangeExtension(fileName, extension);

            DirectoryPath = directoryPath;
            FileName = string.IsNullOrWhiteSpace(fileName) ? defaultFileName : fileName;
            FullPath = Path.Combine(DirectoryPath, FileName);
        }
    }
}