using System.Text;
using System.IO;

namespace Referring.Core
{
    public static class FileManager
    {
        private static readonly Encoding defaultEncoding = Encoding.GetEncoding(1251);
        public static string FileFullPath { get; set; }
        public static string FileName { get; set; }
        public static string FileDirectory
        {
            get
            {
                return FileFullPath.Replace(FileName, "");
            }
        }

        public static string GetContent(string path)
        {
            return File.ReadAllText(path, defaultEncoding);
        }

        public static void SaveContent(string content, string filePath)
        {
            File.WriteAllText(filePath, content, defaultEncoding);
        }

        public static int GetSymbolCount(string content)
        {
            return content.Length;
        }

        public static bool IsExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void Delete(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
