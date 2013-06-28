// This one needs some fixing, basically make it use a session-specific subfolder and delete once the bot exits

using System.Collections.Generic;
using System.IO;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A class that simplifies IO with cache files.
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="cacheDirectory">The directory path to the cache directory to use.</param>
        public CacheManager(string cacheDirectory)
        {
            this.CacheDirectory = new DirectoryInfo(cacheDirectory);
            if (!this.CacheDirectory.Exists) this.CacheDirectory.Create();
            this.DirectorySeparator = Path.DirectorySeparatorChar.ToString();
        }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        public DirectoryInfo CacheDirectory { get; private set; }
        /// <summary>
        /// Gets the directory seperator character.
        /// </summary>
        public string DirectorySeparator { get; private set; }

        /// <summary>
        /// Attempts to get a file in the cache directory.
        /// Returns null if unsuccessful.
        /// </summary>
        /// <param name="fileName">The name of the file. Must include file extension.</param>
        /// <returns></returns>
        public FileInfo GetFile(string fileName)
        {
            foreach (FileInfo fi in this.CacheDirectory.GetFiles())
            {
                if (fi.Name == fileName) return fi;
            }
            return null;
        }
        /// <summary>
        /// Attempts to get a file in the cache directory.
        /// Returns null if unsuccessful.
        /// </summary>
        /// <param name="fileName">The name of the file. Must include file extension.</param>
        /// <param name="subDirectory">The name of the subdirectory. Can contain several subdirectories (i.e. "1\\2\\3").</param>
        /// <returns></returns>
        public FileInfo GetFile(string fileName, string subDirectory)
        {
            string dirPath = this.CacheDirectory.FullName;
            if (!dirPath.EndsWith(this.DirectorySeparator)) dirPath += this.DirectorySeparator; 
            DirectoryInfo di = new DirectoryInfo(dirPath + subDirectory);
            if (!di.Exists) return null;
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Name == fileName) return fi;
            }
            return null;
        }
        /// <summary>
        /// Attempts to get a list of files in the cache directory.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use. By default set to get all files.</param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetFiles(string searchPattern = "*")
        {
            foreach (FileInfo fi in this.CacheDirectory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly))
            {
                yield return fi;
            }
        }
        /// <summary>
        /// Attempts to get a list of files in a subdirectory in the cache directory.
        /// </summary>
        /// <param name="subDirectory">The name of the subdirectory. Can contain several subdirectories (i.e. "1\\2\\3")</param>
        /// <param name="searchPattern">The search pattern to use. By default set to get all files.</param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetFiles(string subDirectory, string searchPattern = "*")
        {
            string dirPath = this.CacheDirectory.FullName;
            if (!dirPath.EndsWith(this.DirectorySeparator)) dirPath += this.DirectorySeparator;
            DirectoryInfo di = new DirectoryInfo(dirPath + subDirectory);
            if (!di.Exists) yield break;

            foreach (FileInfo fi in di.GetFiles(searchPattern, SearchOption.TopDirectoryOnly))
            {
                yield return fi;
            }
        }
        /// <summary>
        /// Writes a file to the cache directory.
        /// Will create subdirectories if specified.
        /// </summary>
        /// <param name="fileName">The name of the file. Must include file extension. Can include subdirectories (i.e. "1\\2\\file.dat").</param>
        /// <param name="data">The data to write to the cache.</param>
        public FileInfo SetFile(string fileName, byte[] data)
        {
            string name = fileName;
            string dirPath = this.CacheDirectory.FullName;
            if (!dirPath.EndsWith(this.DirectorySeparator)) dirPath += this.DirectorySeparator;
            if (fileName.Contains(this.DirectorySeparator.ToString()))
            {
                string[] split = fileName.Split(char.Parse(this.DirectorySeparator));
                for (int i = 0; i < split.Length - 1; i++) dirPath += split[i] + this.DirectorySeparator;
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                name = split[split.Length - 1];
            }
            FileInfo fi = new FileInfo(dirPath + name);
            if (fi.Exists) fi.Delete();
            using (FileStream fstream = fi.Create()) fstream.Write(data, 0, data.Length);
            fi.Refresh();
            return fi;
        }
    }
}
