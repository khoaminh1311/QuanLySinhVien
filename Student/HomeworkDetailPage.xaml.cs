using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation; // QUAN TRỌNG: Dùng để điều hướng Back
using static InterfaceSinhVien.HomeworkPage; // Để dùng class HomeworkItem

namespace InterfaceSinhVien
{
    public partial class HomeworkDetailPage : Page
    {
        private HomeworkItem _currentItem;

        public HomeworkDetailPage(HomeworkItem item)
        {
            InitializeComponent();
            _currentItem = item;

            // Đổ dữ liệu
            txtContent.Text = item.Content;
            txtStartDate.Text = item.StartDate.ToString("dd/MM/yyyy - HH:mm");
            txtEndDate.Text = item.EndDate.ToString("dd/MM/yyyy - HH:mm");

            // Màu sắc trạng thái
            txtStatus.Text = item.StatusText;
            txtStatus.Foreground = item.StatusForeColor;
            borderStatus.Background = item.StatusBgColor;

            CheckSubmitStatus();
        }

        private void CheckSubmitStatus()
        {
            if (_currentItem.SubmitDate != null)
            {
                btnNopBai.Content = "✔ Đã nộp bài";
                btnNopBai.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E"));
                btnNopBai.IsEnabled = false;
            }
            else if (DateTime.Now > _currentItem.EndDate)
            {
                btnNopBai.Content = "⛔ Hết hạn nộp";
                btnNopBai.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                btnNopBai.IsEnabled = false;
            }
        }

        // --- SỰ KIỆN NÚT BACK (QUAN TRỌNG) ---
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang trước đó (Trang danh sách bài tập)
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void BtnNopBai_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng nộp bài đang được phát triển!", "Thông báo");
        }
    }
}