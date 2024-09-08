using System.IO;
using System.Xml.Linq;

namespace Civ6TranslationToolWPF.Levie
{
    public class FileState
    {
        public string FilePath { get; set; }
        public int LastIndex { get; set; }
        public DateTime LastModified { get; set; }
        public XDocument XmlDocument { get; set; }
        public List<TextData> TextDataList { get; set; }
        public bool IsCurrent { get; set; } = false;

        public FileState(string filePath)
        {
            FilePath = filePath;
            XmlDocument = XDocument.Load(filePath);
            TextDataList = new List<TextData>();
            LastIndex = 0;
            LastModified = DateTime.Now;
        }

        public static async Task<FileState> CreateAsync(string filePath)
        {
            var fileState = new FileState(filePath);
            using (var stream = File.OpenRead(filePath))
            {
                fileState.XmlDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            }
            return fileState;
        }

        public FileState()
        {
            FilePath = "";
            TextDataList = new List<TextData>();
            LastIndex = 0;
            LastModified = DateTime.Now;
            XmlDocument = new XDocument();
        }
    }


}
