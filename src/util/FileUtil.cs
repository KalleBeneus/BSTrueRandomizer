using System.IO;

namespace BSTrueRandomizer.util
{
    public class FileUtil
    {
        public static string AddFolderSeparatorIfMissing(string folderPath)
        {
            return Path.TrimEndingDirectorySeparator(folderPath) + Path.DirectorySeparatorChar;
        }
    }
}
