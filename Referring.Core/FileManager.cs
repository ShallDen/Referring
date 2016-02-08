using System.Text;
using System.IO;

namespace Referring.Core
{
    public interface IFileManager
    {
        string GetContent(string filePath);
        void SaveContent(string content, string filePath);
        int GetSymbolCount(string content);
        bool IsExist(string filePath);
        void Delete(string filePath);
    }

    public class FileManager : IFileManager
    {
        private readonly Encoding defaultEncoding = Encoding.GetEncoding(1251);

        public string GetContent(string path)
        {
            return File.ReadAllText(path, defaultEncoding);
        }

        public void SaveContent(string content, string filePath)
        {
            File.WriteAllText(filePath, content, defaultEncoding);
        }

        public int GetSymbolCount(string content)
        {
            return content.Length;
        }

        public bool IsExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
