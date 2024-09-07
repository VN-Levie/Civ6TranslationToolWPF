using Civ6TranslationToolWPF.Levie;
using Civ6TranslationToolWPF.Pages;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace Civ6TranslationToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppState appState = AppState.GetInstance();
        
        public MainWindow()
        {

            InitializeComponent();

            MainFrame.Navigate(new MainPage());
            //xmlTreeView.
        }
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

        // Xử lý thu nhỏ cửa sổ
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Xử lý phóng to hoặc khôi phục cửa sổ
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        // Xử lý đóng cửa sổ
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Di chuyển cửa sổ khi kéo thanh tiêu đề
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

    }


}