using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using Panel = System.Windows.Controls.Panel;
using UserControl = System.Windows.Controls.UserControl;

namespace Civ6TranslationToolWPF
{
    public partial class ToastNotification : UserControl
    {
        private static readonly int MaxLength = 150;
        private DispatcherTimer _timer;
        private bool _isMouseOver;
        private readonly string _fullMessage;

        public ToastNotification(string message)
        {
            InitializeComponent();

            _fullMessage = message;

            if (message.Length > MaxLength)
            {
                NotificationText.Text = message.Substring(0, MaxLength) + "\n... \n"+MainWindow.T("ClickToSeeDetails");
            }
            else
            {
                NotificationText.Text = message;
            }


            this.Height = Double.NaN;

            double time = NotificationText.Text.Length < MaxLength ? 1.5 : 3;


            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(time);
            _timer.Tick += (s, e) =>
            {
                if (!_isMouseOver)
                {
                    StartFadeOutAndClose();  // Gọi hiệu ứng mờ dần trước khi đóng
                    _timer.Stop();
                }
            };
            _timer.Start();
        }


        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOver = true;
            _timer.Stop();
        }


        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOver = false;
            StartFadeOutAndClose();
            _timer.Start();
        }


        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (_fullMessage.Length > MaxLength)
            {
                MessageBox.Show(_fullMessage, MainWindow.T("NotificationDetails"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            StartFadeOutAndClose();

        }


        private void CloseNotification()
        {
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            StartFadeOutAndClose();
        }

        private void StartFadeOutAndClose()
        {
            // Lấy Storyboard từ tài nguyên
            var fadeOutStoryboard = this.FindResource("FadeOutStoryboard") as Storyboard;

            if (fadeOutStoryboard != null)
            {
                fadeOutStoryboard.Completed += (s, e) => CloseNotification(); // Khi hoàn tất hiệu ứng, đóng thông báo
                fadeOutStoryboard.Begin(this);  // Bắt đầu hiệu ứng mờ dần
            }
        }

    }
}
