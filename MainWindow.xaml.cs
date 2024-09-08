using Civ6TranslationToolWPF.Levie;
using Civ6TranslationToolWPF.Pages;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
namespace Civ6TranslationToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public AppState appState = AppState.GetInstance();

        private static readonly Lazy<MainWindow> _instance = new(() => new MainWindow());

        // Thuộc tính Instance để truy cập đối tượng singleton
        public static MainWindow Instance => _instance.Value;
        public MainWindow()
        {

            InitializeComponent();

            DataContext = this;

            var state = new WindowStateManager(this);

            RefreshCommand = new RelayCommand(() => { });
            MinimizeCommand = new RelayCommand(() => state.Minimize());
            MaximizeRestoreCommand = new RelayCommand(() => state.MaximizeRestore());
            CloseCommand = new RelayCommand(() => state.Close());           
            MainFrame.Navigate(new MainPage());

            //xmlTreeView.
        }

        public void ChangeLanguage(string langCode)
        {
           // _ = MainPage.Instance.LoadDataWithProgressAsync(50);
            try
            {
                string languageResources = "Resources/Resources.en.xaml";
                languageResources = langCode switch
                {
                    "en_US" => "Resources/Resources.en.xaml",
                    "vi_VN" => "Resources/Resources.vi.xaml",
                    _ => "Resources.vi.xaml",
                };
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(languageResources, UriKind.Relative);

                if (Application.Current.Resources.MergedDictionaries.Count > 0)
                {
                    Application.Current.Resources.MergedDictionaries[0] = dict;
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                appState.Language = langCode;
                appState.Save();
                // Cập nhật các text động
                MainPage.Instance.UpdateDynamicText();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi thay đổi ngôn ngữ: {ex.Message}", "Error");
            }
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand MinimizeCommand { get; set; }
        public RelayCommand MaximizeRestoreCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        public static string T(string key, params string[] args)
        {
            try
            {
                string? resourceString = Application.Current.FindResource(key) as string;

                if (resourceString != null)
                {
                    return string.Format(resourceString, args);
                }

                return KeyToText(key);
            }
            catch (Exception)
            {
                return KeyToText(key);
            }
        }


        public static string T(string key)
        {
            string? resourceString = Application.Current.FindResource(key) as string;

            return resourceString ?? KeyToText(key);
        }

        public static string KeyToText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;

            string result = Regex.Replace(key, @"[_\-.]", " ");

            StringBuilder formattedText = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                char currentChar = result[i];

                if (char.IsUpper(currentChar))
                {
                    if (i > 0 && !char.IsUpper(result[i - 1]))
                    {
                        formattedText.Append(" ");
                    }
                    else if (i > 0 && char.IsUpper(result[i]) && (i + 1 < result.Length && !char.IsUpper(result[i + 1])))
                    {
                        formattedText.Append(" ");
                    }
                }

                formattedText.Append(currentChar);
            }

            return formattedText.ToString().Trim();
        }

        private void ChangeLanguageToEnglish_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.ShowNotification("Ngôn ngữ đã được chuyển sang tiếng Anh");
            ChangeLanguage("en_US");
        }

        private void ChangeLanguageToVietnamese_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.ShowNotification("Ngôn ngữ đã được chuyển sang tiếng Việt");
            ChangeLanguage("vi_VN");
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       

    }


}