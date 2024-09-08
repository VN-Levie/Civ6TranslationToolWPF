using System.IO;
using System.Windows;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace Civ6TranslationToolWPF
{
    /// <summary>
    /// Interaction logic for DictionaryWindow.xaml
    /// </summary>
    public partial class DictionaryWindow : Window
    {
        public DictionaryWindow()
        {
            InitializeComponent();
        }

        private void SaveDictionary_Click(object sender, RoutedEventArgs e)
        {
            string dictionaryName = dictionaryNameTextBox.Text;
            string dictionaryContent = dictionaryContentTextBox.Text;

            if (string.IsNullOrEmpty(dictionaryName) || string.IsNullOrEmpty(dictionaryContent))
            {
                MessageBox.Show("Tên từ điển và nội dung không được để trống.");
                return;
            }

            SaveDictionary(dictionaryName, dictionaryContent);
        }

        private void SaveDictionary(string dictionaryName, string dictionaryContent)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, "dictionary", "offline", "user");
            Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, dictionaryName + ".json");
            File.WriteAllText(filePath, dictionaryContent);
            MessageBox.Show("Từ điển đã được lưu.");
        }

        private void DeleteDictionary_Click(object sender, RoutedEventArgs e)
        {
            // Xử lý xóa từ điển
            var button = sender as Button;
            var dictionaryName = button.DataContext as string;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "dictionary", "offline", "user", dictionaryName + ".json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                MessageBox.Show("Từ điển đã được xóa.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
