using System;
using System.Windows;
using System.Windows.Input;
using Civ6TranslationToolWPF.Levie;
using Civ6TranslationToolWPF.Pages;
using CommunityToolkit.Mvvm.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Civ6TranslationToolWPF.ViewModel
{
    public class MainWindowViewModel
    {
        


        public readonly MainPage mainPage;
        public readonly AppState appState;
     
       

      
        public ICommand ChangeLanguageToEnglishCommand { get; }
        public ICommand ChangeLanguageToVietnameseCommand { get; }

        public MainWindowViewModel(MainPage mainPage, AppState appState)
        {
           

            this.mainPage = mainPage;
            this.appState = appState;
            

            ChangeLanguageToEnglishCommand = new RelayCommand(() => ChangeLanguage("en_US"));
            ChangeLanguageToVietnameseCommand = new RelayCommand(() => ChangeLanguage("vi_VN"));

            ChangeLanguage(this.appState.Language);

            this.mainPage.LanguageChangeRequested += OnLanguageChangeRequested;
        }

        public void SaveAppState()
        {
            appState.Save();  // Lưu trạng thái ứng dụng
        }

        private void OnLanguageChangeRequested(object sender, string language)
        {
            ChangeLanguage(language);
        }

        public void ChangeLanguage(string langCode)
        {
            try
            {
                string languageResources = langCode switch
                {
                    "en_US" => "Resources/Resources.en.xaml",
                    "vi_VN" => "Resources/Resources.vi.xaml",
                    _ => "Resources/Resources.vi.xaml",
                };

                var dict = new ResourceDictionary { Source = new Uri(languageResources, UriKind.Relative) };

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi thay đổi ngôn ngữ: {ex.Message}", "Error");
            }
            mainPage.UpdateDynamicText();
        }
    }
}
