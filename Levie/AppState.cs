using Newtonsoft.Json;
using System.IO;
namespace Civ6TranslationToolWPF.Levie
{
    public class AppState
    {
        public string Author { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ProjectName { get; set; }
        public string Language { get; set; }
        public FileState LastFile { get; set; }
        public string DirPath { get; set; }
        public bool Opened { get; set; } = false;
        public List<FileState> Histories { get; set; }


        private static readonly string stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appData", "state.json");


        // Biến lưu trữ singleton của AppState
        private static AppState? _instance;

        // Phương thức lấy instance (Singleton)
        public static AppState GetInstance()
        {
            if (_instance == null)
            {
                _instance = Load(); // Tự động khôi phục trạng thái từ file khi khởi tạo
            }
            return _instance;
        }

        // Khởi tạo mặc định
        public AppState()
        {
            Histories = [];
            LastModifiedDate = DateTime.Now;
            LastFile = new FileState();
            Author = "Vn.Levie";
            ProjectName = "Levie's Project";
            DirPath = "";
            Language = "en_US";
        }

        // Phương thức lưu trạng thái vào file JSON
        public void Save()
        {
            try
            {
                Author ??= "Vn.Levie";
                ProjectName ??= "Levie's Project";
                var json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                Directory.CreateDirectory(path: Path.GetDirectoryName(stateFilePath) ?? string.Empty);
                File.WriteAllText(stateFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu trạng thái: {ex.Message}");
            }
        }

        // Phương thức khôi phục trạng thái từ file JSON
        public static AppState Load()
        {
            try
            {
                if (File.Exists(stateFilePath))
                {

                    return JsonConvert.DeserializeObject<AppState>(File.ReadAllText(stateFilePath)) ?? new AppState();


                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi khôi phục trạng thái: {ex.Message}");
            }

            // Trả về trạng thái mặc định nếu không tìm thấy tệp hoặc có lỗi
            return new AppState();
        }

        internal void CheckAndAddOrUpdateLastFile()
        {
            //khởi taọ một List chứa các filestate nếu chưa có thì tạo mới
            Histories ??= [];
            //kiểm tra xem LastFile có tồn tại trong Histories chưa
            var file = Histories.FirstOrDefault(f => f.FilePath  == LastFile.FilePath );
            if (file == null)
            {
                //nếu chưa tồn tại thì thêm vào
                Histories.Add(LastFile);
            }
            else
            {
                //nếu đã tồn tại thì cập nhật lại
                file.LastIndex = LastFile.LastIndex;
                file.LastModified = LastFile.LastModified;
            }
        }

        internal int GetIndex()
        {
            //khởi taọ một List chứa các filestate nếu chưa có thì tạo mới
            Histories ??= [];
            //kiểm tra xem LastFile có tồn tại trong Histories chưa
            var file = Histories.FirstOrDefault(f => f.FilePath  == LastFile.FilePath );
            if (file == null)
            {
                //nếu chưa tồn tại thì thêm vào
                Histories.Add(LastFile);
            }

            return LastFile.LastIndex;
        }

        // Cập nhật thông tin file cuối cùng đã mở

    }
}
