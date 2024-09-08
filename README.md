# Civ6 Translation Tool

🇬🇧 [**English**](#english)  
🇻🇳 [**Tiếng Việt**](#vietnamese)


## <a name="english"></a> English

### Civ6 Translation Tool

The **Civ6 Translation Tool** is a WPF application designed to help with translating XML files for the game **Civilization VI**. The tool offers an intuitive interface to open, edit, and save translation files, including features like XML file tree views, search, text editing for original and translated text, and translation assistance tools.

### Features

- Open and display XML files in a TreeView.
- Edit original and translated text with features like copy, paste, and auto-translate.
- Search within XML files by keywords.
- Save changes and manage translation files efficiently.
- Progress bar for indicating the loading and saving process.
- Multi-language UI support (English and Vietnamese).
- Custom title bar with enhanced window controls (thanks to [Morten Brudvik](https://github.com/mortenbrudvik/custom-wpf-window-titlebar-with-snap-layout) and the **ControlzEx** library).

### System Requirements

- .NET Framework >= 4.7.2 or .NET Core >= 3.1
- Windows 7 or higher

### Installation

1. Clone the project:

   ```bash
   git clone https://github.com/VN-Levie/Civ6TranslationToolWPF.git
   cd Civ6TranslationToolWPF
   ```

2. Open the project in Visual Studio or any .NET-supported IDE.

3. Install dependencies via NuGet:

   ```bash
   Install-Package AvalonEdit
   Install-Package MaterialDesignThemes
   ```

4. Build and run the project.

### Usage

1. Open the application and select the folder containing the XML files by clicking the **Select Folder** button.
2. Select the XML file you wish to translate from the tree view on the left.
3. Enter the translated text and click **Save**.
4. Use the **Auto-Translate** feature to quickly translate text.
5. Search for specific text using the search bar.

### Acknowledgements

- **ControlzEx**: This library provided the base framework for the custom title bar and enhanced window controls.
- Special thanks to [Morten Brudvik](https://github.com/mortenbrudvik/custom-wpf-window-titlebar-with-snap-layout) for his repository, which helped in the implementation of the custom title bar.
- **CommunityToolkit**: The community behind this toolkit has provided valuable tools and resources that enhanced the development process.

### License

This project is licensed under the **GNU General Public License v3.0**. You are free to download, modify, and distribute the code, but any distributed versions must remain under the GPL v3.0 license, and the source code must be made available.

See the full license [here](https://www.gnu.org/licenses/gpl-3.0.en.html).

---

## <a name="vietnamese"></a> Tiếng Việt

### Công cụ dịch Civ6

**Công cụ dịch Civ6** là một ứng dụng WPF giúp dịch các tệp XML cho trò chơi **Civilization VI**. Công cụ này cung cấp giao diện trực quan để mở, chỉnh sửa, và lưu các tệp dịch, bao gồm các tính năng như hiển thị cây thư mục XML, tìm kiếm, chỉnh sửa văn bản gốc và dịch, cùng với các công cụ hỗ trợ dịch.

### Các tính năng

- Mở và hiển thị các tệp XML dưới dạng cây (TreeView).
- Chỉnh sửa văn bản gốc và văn bản dịch với các tính năng như sao chép, dán, và dịch tự động.
- Tìm kiếm trong các tệp XML bằng từ khóa.
- Lưu thay đổi và quản lý các tệp dịch một cách hiệu quả.
- Thanh tiến trình để hiển thị quá trình tải và lưu tệp.
- Hỗ trợ giao diện đa ngôn ngữ (tiếng Anh và tiếng Việt).
- Thanh tiêu đề tùy chỉnh với các điều khiển cửa sổ nâng cao (cảm ơn [Morten Brudvik](https://github.com/mortenbrudvik/custom-wpf-window-titlebar-with-snap-layout) và thư viện **ControlzEx**).

### Yêu cầu hệ thống

- .NET Framework >= 4.7.2 hoặc .NET Core >= 3.1
- Hệ điều hành: Windows 7 trở lên

### Hướng dẫn cài đặt

1. Clone dự án:

   ```bash
   git clone https://github.com/VN-Levie/Civ6TranslationToolWPF.git
   cd Civ6TranslationToolWPF
   ```

2. Mở dự án trong Visual Studio hoặc bất kỳ IDE nào hỗ trợ .NET.

3. Cài đặt các gói phụ thuộc qua NuGet:

   ```bash
   Install-Package AvalonEdit
   Install-Package MaterialDesignThemes
   ```

4. Build và chạy dự án.

### Cách sử dụng

1. Mở ứng dụng và chọn thư mục chứa các tệp XML bằng cách nhấn vào nút **Chọn thư mục**.
2. Chọn tệp XML cần dịch từ cây thư mục bên trái.
3. Nhập văn bản dịch vào ô và nhấn **Lưu**.
4. Sử dụng tính năng **Dịch tự động** để dịch nhanh văn bản.
5. Tìm kiếm văn bản bằng thanh tìm kiếm.

### Cảm ơn

- **ControlzEx**: Thư viện này cung cấp nền tảng cho việc tùy chỉnh thanh tiêu đề và các điều khiển cửa sổ nâng cao.
- Đặc biệt cảm ơn [Morten Brudvik](https://github.com/mortenbrudvik/custom-wpf-window-titlebar-with-snap-layout) với repository đã giúp thực hiện tính năng thanh tiêu đề tùy chỉnh.
- **CommunityToolkit**: Cộng đồng đã cung cấp các công cụ và tài nguyên hữu ích, giúp quá trình phát triển diễn ra thuận lợi hơn.

### Giấy phép

Dự án này được cấp phép theo **GNU General Public License v3.0**. Bạn có thể tải về, chỉnh sửa và phân phối mã nguồn, nhưng mọi phiên bản phân phối phải giữ nguyên giấy phép GPL v3.0 và mã nguồn phải được công khai.

Xem chi tiết giấy phép [tại đây](https://www.gnu.org/licenses/gpl-3.0.vi.html).



