using System.IO;
using System.Xml.Linq;

namespace Civ6TranslationToolWPF.Levie
{
    public class FileState
    {
        public string FilePath { get; set; } = string.Empty;
        public int LastIndex { get; set; } = 0;
        public DateTime LastModified { get; set; }
        public XDocument XmlDocument { get; set; } = new();
        public List<TextData> TextDataList { get; set; } = [];
        public bool IsCurrent { get; set; } = false;

        public FileState(string filePath)
        {
            FilePath = filePath;
            XmlDocument = XDocument.Load(filePath);
            TextDataList = [];
            LastIndex = 0;
            LastModified = DateTime.Now;
        }


        public static async Task<FileState> CreateAsync(string filePath)
        {
            var fileState = new FileState();
            using (var stream = File.OpenRead(filePath))
            {
                fileState.XmlDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            }
           
            return fileState;
        }

        public FileState()
        {
         
            LastIndex = 0;
            LastModified = DateTime.Now;
            XmlDocument = new XDocument();
        }
    }


}
