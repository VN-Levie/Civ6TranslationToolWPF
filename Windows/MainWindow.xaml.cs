﻿using Civ6TranslationToolWPF.Levie;
using Civ6TranslationToolWPF.Pages;
using Civ6TranslationToolWPF.ViewModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.DataFormats;
using Application = System.Windows.Application;
using static System.Windows.Visibility;
using static System.Windows.WindowState;
using MessageBox = System.Windows.MessageBox;
using Civ6TranslationToolWPF.Windows;
using System.Runtime.CompilerServices;
namespace Civ6TranslationToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {


        private LoadingWindow _loadingWindow;  // Lưu trữ loadingWindow
        public MainWindow(MainWindowViewModel viewModel, LoadingWindow loading)
        {
            _loadingWindow = loading;
           
            InitializeComponent();
           
            DataContext = viewModel;
           
            viewModel.mainPage.PageLoaded += MainPage_PageLoaded!;  // Đăng ký event
          
           MainFrame.Navigate(viewModel.mainPage);        

            this.StateChanged += (_, _) => RefreshMaximizeRestoreButton();

            RefreshMaximizeRestoreButton();

            
        }

        private void MainPage_PageLoaded(object sender, EventArgs e)
        {
            _loadingWindow?.Close();
        }


        private bool IsMaximized => this.WindowState == Maximized;



        public void MaximizeRestore() => this.WindowState = IsMaximized ? Normal : Maximized;

        private void RefreshMaximizeRestoreButton()
        {
            MaximizeButton.Visibility = IsMaximized ? Collapsed : Visible;
            RestoreButton.Visibility = IsMaximized ? Visible : Collapsed;
        }

        
      
    
      

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = Minimized;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = Normal;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = Maximized;
        }
    }

}