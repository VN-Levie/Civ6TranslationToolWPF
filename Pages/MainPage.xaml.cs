using Civ6TranslationToolWPF.Levie;
using MaterialDesignThemes.Wpf;
using System.Collections.Concurrent;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Clipboard = System.Windows.Clipboard;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
namespace Civ6TranslationToolWPF.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {


        private string _currentFilePath = String.Empty;
        private string _currentFolderPath = String.Empty;
        //private readonly Dictionary<string, FileState> openFiles = [];
        private readonly ConcurrentDictionary<string, FileState> openFiles = [];
        public bool IsLoadDone { get; set; } = false;

        private readonly AppState _appState;
        public event EventHandler<string> LanguageChangeRequested = delegate { };
        public event EventHandler PageLoaded = delegate { };
        public MainPage(AppState appState)
        {

            InitializeComponent();
            _appState = appState;
            _ = LoadDataWithProgressAsync(LockXml: true, NeedWait: true, LockControll: true);

            IsLoadDone = true;
            this.SizeChanged += MainPage_SizeChanged;

            InitializeAppState();
            this.Loaded += MainPage_Loaded;

        }
        // Khi trang được tải, kích hoạt event
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageLoaded?.Invoke(this, EventArgs.Empty);  // Gọi event
        }

        private void InitializeAppState()
        {
            _currentFilePath = _appState.LastFile.FilePath;
            _currentFolderPath = _appState.DirPath;
            if (!String.IsNullOrEmpty(_currentFilePath))
            {
                _ = LoadXmlDataToDataGridAsync(_currentFilePath);
                //_openTask.RunSynchronously();
            }
            if (!String.IsNullOrEmpty(_currentFolderPath))
            {
                //_currentFolderPath = String.Empty;
                if (Path.GetFileName(_currentFolderPath).Equals("DLC"))
                {
                    LoadDLCXmlFilesToTreeView(_currentFolderPath);
                }
                else
                {
                    LoadXmlFilesToTreeView(_currentFolderPath);
                }
            }
            UnsavedChangesWarningTextBlock.Text = string.Format(T("UnsavedChangesWarning"), T("SaveFile"));
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e.NewSize.Width < 800)
            {
                TreeViewColumn.Width = new GridLength(0);
                TreeViewExpander.IsExpanded = false;
                ToggleTreeViewIcon.Kind = PackIconKind.MenuClose;
            }
            else
            {
                TreeViewColumn.Width = new GridLength(300);
                TreeViewExpander.IsExpanded = true;
                ToggleTreeViewIcon.Kind = PackIconKind.Backburger;
            }
        }

        private void ToggleTreeView_Click(object sender, RoutedEventArgs e)
        {
            if (TreeViewExpander.IsExpanded)
            {
                TreeViewColumn.Width = new GridLength(0);
                TreeViewExpander.IsExpanded = false;
                ToggleTreeViewIcon.Kind = PackIconKind.MenuClose;
            }
            else
            {
                TreeViewColumn.Width = new GridLength(300);
                TreeViewExpander.IsExpanded = true;

                ToggleTreeViewIcon.Kind = PackIconKind.Backburger;
            }
        }

        public async Task LoadDataWithProgressAsync(int totalSteps = 100, bool NeedWait = false, bool LockControll = false, bool LockXml = false)
        {

            IsLoadDone = false;
            progressText.Text = "Starting";
            progressBar.Visibility = Visibility.Visible;
            progressText.Visibility = Visibility.Visible;

            if (LockXml)
            {
                xmlTreeView.IsEnabled = false;
                OpenXMLFile_btn.IsEnabled = false;
            }
            if (LockControll)
            {
                DisableController();
            }

            progressBar.Maximum = totalSteps;

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                progressText.Text = $"Loading... {value}%";
            });


            await Task.Run(() =>
            {
                for (int i = 0; i <= totalSteps; i++)
                {
                    Task.Delay(5).Wait();

                    (progress as IProgress<int>)?.Report(i);
                }
            });

            while (!IsLoadDone && NeedWait)
            {
                await Task.Delay(5);
            }

            progressBar.Value = totalSteps;
            progressText.Text = "Loading completed!";
            if (LockXml)
            {
                xmlTreeView.IsEnabled = true;
                OpenXMLFile_btn.IsEnabled = true;
            }
            if (LockControll)
            {
                EnableController();
            }
            await Task.Delay(1000);
            progressBar.Visibility = Visibility.Collapsed;
            progressText.Visibility = Visibility.Collapsed;


        }




        public void UpdateDynamicText()
        {
            string unsavedChangesWarning = string.Format(T("UnsavedChangesWarning"), T("SaveFile"));

            UnsavedChangesWarningTextBlock.Text = unsavedChangesWarning;
        }



        private void OpenBaseFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = T("SelectXmlFolder");
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;

                    if (Path.GetFileName(selectedPath) == "DLC")
                    {
                        LoadDLCXmlFilesToTreeView(selectedPath);
                    }
                    else
                    {
                        LoadXmlFilesToTreeView(selectedPath);
                    }
                    foreach (TreeViewItem item in xmlTreeView.Items)
                    {
                        if (item.Tag.ToString() == _currentFilePath)
                        {
                            item.IsExpanded = true;
                            item.Focus();
                            item.BringIntoView();
                            break;
                        }
                    }
                    _appState.DirPath = _currentFilePath = selectedPath;
                    _appState.Save();
                }
            }
        }

        private void LoadDirectoryToTreeView(TreeViewItem parentItem, string folderPath)
        {

            var xmlFiles = Directory.GetFiles(folderPath, "*.xml");
            foreach (var file in xmlFiles)
            {
                TreeViewItem xmlItem = new TreeViewItem
                {
                    Header = Path.GetFileName(file),
                    Tag = file
                };


                xmlItem.Selected += XmlItem_Selected;

                parentItem.Items.Add(xmlItem);
                if (file == _currentFilePath)
                {


                    xmlItem.IsSelected = true;

                    xmlItem.IsExpanded = true;
                    xmlItem.Focus();
                    xmlItem.BringIntoView();
                }
            }

            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                TreeViewItem directoryItem = new TreeViewItem
                {
                    Header = Path.GetFileName(directory),
                    Tag = directory
                };

                LoadDirectoryToTreeView(directoryItem, directory);

                parentItem.Items.Add(directoryItem);
            }
        }

        private void LoadXmlFilesToTreeView(string folderPath)
        {
            try
            {
                xmlTreeView.Items.Clear();

                TreeViewItem rootItem = new TreeViewItem
                {
                    Header = Path.GetFileName(folderPath),
                    Tag = folderPath
                };

                LoadDirectoryToTreeView(rootItem, folderPath);

                xmlTreeView.Items.Add(rootItem);
                rootItem.IsExpanded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(T("ErrorLoadingXmlFile", folderPath, ex.Message));
            }
        }

        private void LoadDLCXmlFilesToTreeView(string dlcFolderPath)
        {
            try
            {
                xmlTreeView.Items.Clear();

                TreeViewItem dlcRootItem = new TreeViewItem
                {
                    Header = "DLC",
                    Tag = dlcFolderPath
                };

                var dlcDirectories = Directory.GetDirectories(dlcFolderPath);

                foreach (var dlcDirectory in dlcDirectories)
                {
                    string textFolderPath = Path.Combine(dlcDirectory, "Text");
                    if (Directory.Exists(textFolderPath))
                    {
                        TreeViewItem dlcItem = new TreeViewItem
                        {
                            Header = Path.GetFileName(dlcDirectory),
                            Tag = dlcDirectory
                        };

                        var xmlFiles = Directory.GetFiles(textFolderPath, "*.xml");
                        foreach (var xmlFile in xmlFiles)
                        {
                            TreeViewItem xmlItem = new TreeViewItem
                            {
                                Header = Path.GetFileName(xmlFile),
                                Tag = xmlFile
                            };


                            dlcItem.Items.Add(xmlItem);
                            if (xmlItem.Tag.ToString() == _currentFilePath)
                            {

                                xmlItem.IsSelected = true;

                                xmlItem.IsExpanded = true;
                                xmlItem.Focus();
                                xmlItem.BringIntoView();
                            }
                        }

                        dlcRootItem.Items.Add(dlcItem);
                        dlcItem.IsExpanded = true;
                    }
                }

                xmlTreeView.Items.Add(dlcRootItem);
                dlcRootItem.IsExpanded = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show(T("ErrorLoadingDLCXmlFile", ex.Message));
            }
        }


        private async void XmlItem_Selected(object sender, RoutedEventArgs e)
        {


            if (sender is TreeViewItem selectedItem)
            {
                if (selectedItem.Tag is string filePath)
                {
                    if (_currentFilePath != filePath)
                    {

                        await LoadXmlDataToDataGridAsync(filePath);
                    }

                }
            }
            else
            {
                MessageBox.Show(T("ErrorLoadingXmlFile"));
            }

        }
        private void LoadXmlToAvalonEditor(string xmlContent)
        {
            xmlTextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("XML");
            xmlTextEditor.Text = xmlContent;

        }
        private async Task LoadXmlDataToDataGridAsync(string filePath)
        {


            if (string.IsNullOrEmpty(filePath))
            {
                IsLoadDone = true;
                return;
            }

            IsLoadDone = false;
            //xmlTreeView.IsEnabled = false;


            _ = LoadDataWithProgressAsync(LockXml: true, NeedWait: true, LockControll: true);

            try
            {
                FileState newFileState = await FileState.CreateAsync(filePath);
                openFiles.AddOrUpdate(filePath, newFileState, (key, existingValue) => newFileState);

                FileState? currentFileState = openFiles[filePath];

                if (currentFileState == null)
                {
                    await LoadXmlDataToDataGridAsync(filePath);
                    return;
                }

                //LoadXmlToAvalonEditor(await File.ReadAllTextAsync(filePath));
                LoadXmlToAvalonEditor(currentFileState.XmlDocument.ToString());

                var replaceElements = currentFileState.XmlDocument.Descendants("Replace");
                if (!replaceElements.Any())
                {
                    replaceElements = currentFileState.XmlDocument.Descendants("Row");
                }

                if (!replaceElements.Any())
                {
                    xmlTextEditor.IsReadOnly = true;
                    translatedTextBox.IsReadOnly = true;
                    translationDataGrid.ItemsSource = null;

                    currentFileState.TextDataList.Clear();

                    currentFileState.TextDataList.Add(new TextData
                    {
                        Index = 1,
                        OriginalText = T("FileEditingNotSupported"),
                        Tag = "",
                        Language = "",
                        TranslatedText = T("FileEditingNotSupported")
                    });

                    translationDataGrid.ItemsSource = currentFileState.TextDataList;
                    translationDataGrid.SelectedIndex = 0;
                    DisableController();
                    OpenXMLFile_btn.IsEnabled = true;
                    xmlTreeView.IsEnabled = true;
                    IsLoadDone = true;
                    return;
                }

                currentFileState.TextDataList.Clear();
                int index = 1;
                foreach (var item in replaceElements)
                {
                    var tagValue = item.Attribute("Tag")?.Value;
                    var lang = item.Attribute("lang")?.Value ?? item.Attribute("Language")?.Value;
                    var textElement = item.Element("Text");

                    var originalText = item.Value;
                    if (textElement != null)
                    {
                        originalText = textElement.Value;
                    }

                    var translatedText = item.Value;
                    currentFileState.TextDataList.Add(new TextData
                    {
                        Index = index++,
                        OriginalText = originalText,
                        Tag = tagValue ?? "",
                        Language = lang ?? "",
                        TranslatedText = originalText
                    });


                }

                translationDataGrid.ItemsSource = currentFileState.TextDataList;
                translationDataGrid.SelectedIndex = 0;

                EnableController();
                _currentFilePath = filePath;

                _appState.LastFile = currentFileState;
                _appState.LastFile.FilePath = filePath;
                //_appState.CheckAndAddOrUpdateLastFile();

                filePathTextBox.Text = filePath;
                ShowNotification($"currentFileState.LastIndex: {currentFileState.LastIndex}");
                if (currentFileState.LastIndex > 0 && currentFileState.LastIndex <= translationDataGrid.Items.Count)
                {
                    translationDataGrid.SelectedIndex = currentFileState.LastIndex - 1;
                    translationDataGrid.ScrollIntoView(translationDataGrid.SelectedItem);
                }
                else
                {
                    translationDataGrid.SelectedIndex = 0;
                    translationDataGrid.ScrollIntoView(translationDataGrid.SelectedItem);
                }


                // Enable lại các thành phần khi hoàn tất
                xmlTreeView.IsEnabled = true;
                // OpenXMLFile_btn.IsEnabled = true;
                IsLoadDone = true;
                //_appState.Save();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(T("ErrorReadingFile", ex.Message + $"\n{filePath}"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IsLoadDone = true;

        }


        private static string T(string key) => ResourceHelper.T(key);
        private static string T(string key, params string[] args) => ResourceHelper.T(key, args);



        void DisableController()
        {
            xmlTextEditor.IsReadOnly = true;
            translatedTextBox.IsReadOnly = true;

            TransferText_btn.IsEnabled = false;
            Insert_btn.IsEnabled = false;
            AutoTranslate_btn.IsEnabled = false;
            ClearTranslatedText_btn.IsEnabled = false;
            PasteFromClipboard_btn.IsEnabled = false;
            SaveXml_btn.IsEnabled = false;
            FormatDocument_btn.IsEnabled = false;
            WordWrap_checkBox.IsEnabled = false;
        }

        void EnableController()
        {
            xmlTextEditor.IsReadOnly = false;
            translatedTextBox.IsReadOnly = false;

            TransferText_btn.IsEnabled = true;
            Insert_btn.IsEnabled = true;
            AutoTranslate_btn.IsEnabled = true;
            ClearTranslatedText_btn.IsEnabled = true;
            PasteFromClipboard_btn.IsEnabled = true;
            SaveXml_btn.IsEnabled = true;
            FormatDocument_btn.IsEnabled = true;
            WordWrap_checkBox.IsEnabled = true;
        }

        private void OpenDLCFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Select DLC Folder";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                System.Windows.MessageBox.Show($"DLC Folder: {selectedPath}");
            }
        }



        private async void OpenXMLFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                await LoadXmlDataToDataGridAsync(selectedFile);


            }
        }




        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DoClosing();
        }

        void DoClosing()
        {
            _appState.Save();
            //this.Close();
        }

        private void TransferText_Click(object sender, RoutedEventArgs e)
        {
            translatedTextBox.Text = originalTextBox.Text;
        }

        private void CopyOriginalText_Click(object sender, RoutedEventArgs e)
        {

            Clipboard.SetText(originalTextBox.Text);
            ShowNotification(T("Copied"));

        }

        private void AutoTranslate_Click(object sender, RoutedEventArgs e)
        {
            translatedTextBox.Text = "Dịch tự động: " + originalTextBox.Text;
            ShowNotification(T("Translated"));
        }

        private void ClearTranslatedText_Click(object sender, RoutedEventArgs e)
        {
            translatedTextBox.Clear();
        }





        private void TranslationDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (translationDataGrid.SelectedItem is TextData selectedItem && openFiles.TryGetValue(_currentFilePath, out var fileState))
            {
                string teg = selectedItem.Tag.ToLower().Replace("xml.", "").Replace("xml", "");

                var element = fileState.XmlDocument.Descendants()
                                   .FirstOrDefault(x => x.Attribute("Tag")?.Value.ToLower() == selectedItem.Tag);
                if (element != null)
                {
                    int offset = GetOffset(element);
                    _appState.LastFile.LastIndex = selectedItem.Index;
                    if (offset >= 0 && offset < xmlTextEditor.Text.Length)
                    {
                        xmlTextEditor.Select(offset, 0);
                        xmlTextEditor.ScrollToLine(xmlTextEditor.Document.GetLineByOffset(offset).LineNumber);
                        //xmlTextEditor.Focus();
                        translatedTextBox.Focus();
                    }
                    else
                    {
                        ShowNotification(T("XmlElementNotFoundWithTag", selectedItem.Tag));
                    }
                }
            }
        }


        private void TranslationDataGrid_MouseDoubleClick_old(object sender, MouseButtonEventArgs e)
        {
            if (translationDataGrid.SelectedItem is TextData selectedItem)
            {
                var hit = e.OriginalSource as DependencyObject;

                while (hit != null && hit is not DataGridCell)
                {
                    hit = VisualTreeHelper.GetParent(hit);
                }


                translatedTextBox.Text = originalTextBox.Text = selectedItem.OriginalText;
                translatedTextBox.Focus();

                _appState.LastFile.LastIndex = selectedItem.Index;

            }
        }
        private int GetOffset(XElement element)
        {

            string xmlText = xmlTextEditor.Text;

            string elementString = element.ToString(SaveOptions.DisableFormatting);

            int offset = xmlText.IndexOf(elementString, StringComparison.Ordinal);

            return offset;
        }





        public void ShowNotification(string message)
        {
            ToastNotification toast = new ToastNotification(message);

            toast.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            toast.VerticalAlignment = VerticalAlignment.Bottom;
            toast.Margin = new Thickness(10, 10, 10, 10);


            MainGrid.Children.Add(toast);
        }


        private int currentSearchIndex = -1;

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchNextResult();
            }
        }

        private void SearchNextResult()
        {
            string searchText = searchTextBox.Text.Replace("_", " ");

            bool caseSensitive = caseSensitiveCheckBox.IsChecked == true;

            bool tagOnly = tagOnlyCheckBox.IsChecked == true;

            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }
            foreach (var item in translationDataGrid.Items)
            {
                TextData? textData = item as TextData;
                if (textData != null)
                {
                    textData.IsHighlighted = false;
                }
            }

            for (int i = currentSearchIndex + 1; i < translationDataGrid.Items.Count; i++)
            {
                if (translationDataGrid.Items[i] is TextData textData)
                {
                    bool found;

                    if (tagOnly)
                    {
                        found = caseSensitive
                            ? textData.Tag.Replace("_", " ").Contains(searchText)
                            : textData.Tag.Replace("_", " ").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    else
                    {
                        found = caseSensitive
                            ? textData.Tag.Replace("_", " ").Contains(searchText) || textData.OriginalText.Contains(searchText)
                            : textData.Tag.Replace("_", " ").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                              textData.OriginalText.Replace("_", " ").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    if (found)
                    {
                        translationDataGrid.ScrollIntoView(textData);
                        translationDataGrid.SelectedItem = textData;
                        textData.IsHighlighted = true;
                        currentSearchIndex = i;
                        return;
                    }
                    else
                    {
                        textData.IsHighlighted = false;
                    }
                }
            }

            currentSearchIndex = -1;
        }



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            string searchText = searchTextBox.Text.Replace("_", " ");

            bool caseSensitive = caseSensitiveCheckBox.IsChecked == true;
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }
            foreach (var item in translationDataGrid.Items)
            {
                TextData? textData = item as TextData;
                if (textData != null)
                {
                    textData.IsHighlighted = false;
                }
            }
            foreach (var item in translationDataGrid.Items)
            {
                TextData? textData = item as TextData;
                if (textData != null)
                {
                    bool found = caseSensitive
                        ? textData.Tag.Replace("_", " ").Contains(searchText) || textData.OriginalText.Contains(searchText)
                        : textData.Tag.Replace("_", " ").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                          textData.OriginalText.Replace("_", " ").IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (found)
                    {
                        translationDataGrid.ScrollIntoView(item);
                        translationDataGrid.SelectedItem = item;
                        textData.IsHighlighted = true;
                        return;
                    }
                    else
                    {
                        textData.IsHighlighted = false;

                    }
                }
            }
        }

        private void TagOnlyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            caseSensitiveCheckBox.IsChecked = false;
            searchTextBox.Focus();
            SearchNextResult();
        }

        private void TagOnlyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            caseSensitiveCheckBox.IsEnabled = true;
            searchTextBox.Focus();
            SearchNextResult();
        }

        private void CaseSensitiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
            SearchNextResult();
        }


        private void CaseSensitiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
            SearchNextResult();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == Key.O)
                {
                    OpenXMLFile_Click(sender, e);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.F)
                {
                    searchTextBox.Focus();
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.O)
                {
                    OpenBaseFolder_Click(sender, e);
                    e.Handled = true;
                    return;
                }
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            int lastIndex = _appState.LastFile.LastIndex;

            TextData? selectedData = translationDataGrid.SelectedItem as TextData;

            if (selectedData == null && lastIndex >= 0 && lastIndex < translationDataGrid.Items.Count)
            {
                selectedData = translationDataGrid.Items[lastIndex] as TextData;
            }

            if (selectedData != null)
            {
                string translatedText = translatedTextBox.Text;
                string tag = selectedData.Tag;

                if (openFiles.TryGetValue(_currentFilePath, out FileState? fileState))
                {
                    var element = fileState.XmlDocument.Descendants()
                                                       .FirstOrDefault(x => x.Attribute("Tag")?.Value == tag);
                    if (element != null)
                    {
                        var textElement = element.Element("Text");

                        if (textElement != null)
                        {

                            textElement.Value = translatedText;

                            selectedData.OriginalText = translatedText;

                            RefreshXmlViewer(fileState);
                            translationDataGrid.SelectedIndex = lastIndex - 1;


                            // Lưu lại trạng thái thay đổi
                            _appState.Save();
                        }
                        else
                        {
                            ShowNotification(T("XmlTextElementNotFound"));
                        }
                    }
                    else
                    {
                        ShowNotification(T("XmlElementNotFound"));
                    }
                }
            }
            else
            {
                ShowNotification(T("SelectDataToSave"));
            }
        }



        private void RefreshXmlViewer(FileState fileState)
        {
            xmlTextEditor.Text = fileState.XmlDocument.ToString(SaveOptions.None);
        }

        private void FormatDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var xmlContent = xmlTextEditor.Text;

                XDocument xmlDocument = XDocument.Parse(xmlContent);

                string formattedXml = xmlDocument.ToString(SaveOptions.None);

                xmlTextEditor.Text = formattedXml;

            }
            catch (Exception ex)
            {

                MessageBox.Show(T("XmlFormatError", ex.Message));
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (openFiles.TryGetValue(_currentFilePath, out var fileState))
            {
                SaveFile(fileState);
                //ShowNotification(T("FileSaveSuccess", Path.GetFileName(fileState.FilePath)));
                ShowNotification(T("FileSaveSuccess", "ok"));
            }
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var fileState in openFiles.Values)
            //{
            //    SaveFile(fileState);
            //}
            ShowNotification(T("SaveAllSuccess"));
        }

        private void SaveFile(FileState fileState)
        {
            try
            {
                fileState.XmlDocument.Save(fileState.FilePath);
            }
            catch (Exception ex)
            {
                ShowNotification(T("FileSaveError", fileState.FilePath, ex.Message));
            }
        }

        private void SaveXml_Click(object sender, RoutedEventArgs e)
        {
            if (openFiles.TryGetValue(_currentFilePath, out var fileState))
            {
                try
                {
                    fileState.XmlDocument = XDocument.Parse(xmlTextEditor.Text);

                    UpdateDataGridFromXml(fileState);

                    ShowNotification(T("XmlSavedSuccessfully"));
                }
                catch (Exception ex)
                {
                    ShowNotification(T("XmlSaveError", ex.Message));
                }
            }
        }

        private void UpdateDataGridFromXml(FileState fileState)
        {
            var replaceElements = fileState.XmlDocument.Descendants("Replace")
                                   .Where(x => x.Attribute("Tag") != null)
                                   .ToList();

            if (replaceElements.Count == 0)
            {
                replaceElements = fileState.XmlDocument.Descendants("Row")
                                     .Where(x => x.Attribute("Tag") != null)
                                     .ToList();
            }



            fileState.TextDataList.Clear();
            int index = 1;

            foreach (var element in replaceElements)
            {
                var tagValue = element.Attribute("Tag")?.Value;
                var lang = element.Attribute("lang")?.Value ?? element.Attribute("Language")?.Value;
                var textElement = element.Element("Text");

                var originalText = element.Value;
                if (textElement != null)
                {
                    originalText = textElement.Value;
                }


                var translatedText = element.Value;
                fileState.TextDataList.Add(new TextData
                {
                    Index = index++,
                    OriginalText = originalText,
                    Tag = tagValue ?? "",
                    Language = lang ?? "",
                    TranslatedText = originalText
                });
            }

            translationDataGrid.ItemsSource = null;
            translationDataGrid.ItemsSource = fileState.TextDataList;
        }


        private void TranslationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (translationDataGrid.SelectedItem is TextData selectedItem && openFiles.TryGetValue(_currentFilePath, out FileState? fileState))
            {
                originalTextBox.Text = selectedItem.OriginalText;
                translatedTextBox.Text = selectedItem.TranslatedText;

                var element = fileState.XmlDocument.Descendants()
                                   .FirstOrDefault(x => x.Attribute("Tag")?.Value == selectedItem.Tag);
                if (element != null)
                {
                    int lineNumber = GetLineNumber(element);

                    xmlTextEditor.ScrollToLine(lineNumber);
                    xmlTextEditor.Select(xmlTextEditor.Document.GetLineByNumber(lineNumber).Offset, 0);
                    xmlTextEditor.Focus();
                }

                fileState.LastIndex = selectedItem.Index;
                _appState.LastFile.LastIndex = selectedItem.Index;
                _appState.Save();
            }
        }

        private int GetLineNumber(XElement element)
        {
            var xmlLines = xmlTextEditor.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int lineNumber = 1;
            string elementString = element.ToString();

            foreach (var line in xmlLines)
            {
                if (line.Contains(element.Name.LocalName) && line.Contains(element.Attribute("Tag")?.Value ?? "_"))
                {
                    return lineNumber + 1;
                }
                lineNumber++;
            }

            return 1; // Mặc định
        }

        private void XmlTextEditor_TextChanged(object sender, EventArgs e)
        {

            if (openFiles.TryGetValue(_currentFilePath, out var fileState))
            {
                try
                {
                    fileState.XmlDocument = XDocument.Parse(xmlTextEditor.Text);
                    UpdateDataGridFromXml(fileState);
                }
                catch
                {
                    //MessageBox.Show(T("XmlParseError"));
                }
            }
        }

        private void WordWrap_Checked(object sender, RoutedEventArgs e)
        {
            xmlTextEditor.WordWrap = true;
        }

        private void WordWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            xmlTextEditor.WordWrap = false;
        }

        private void PasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {

            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();

                translatedTextBox.Text = clipboardText;

                ShowNotification(T("PastedFromClipboard"));
            }
            else
            {
                ShowNotification(T("NoTextInClipboard"));
            }
        }



        private void ChangeLanguageToEnglish_Click(object sender, RoutedEventArgs e) => LanguageChangeRequested?.Invoke(this, "en_US");

        private void ChangeLanguageToVietnamese_Click(object sender, RoutedEventArgs e) => LanguageChangeRequested?.Invoke(this, "vi_VN");




        private void OpenDictionary_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ quản lý từ điển
            DictionaryWindow dictionaryWindow = new DictionaryWindow();
            dictionaryWindow.ShowDialog();
        }
    }
}
