using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // Dùng để điều hướng

namespace InterfaceSinhVien
{
    public partial class NotificationDetailPage : Page
    {
        // Constructor nhận dữ liệu
        public NotificationDetailPage(NotificationItem item)
        {
            InitializeComponent();

            // Đổ dữ liệu lên giao diện
            txtTitle.Text = item.Title;
            txtContent.Text = item.Content;
            txtDate.Text = item.Date.ToString("dd/MM/yyyy HH:mm");
        }

        // Sự kiện nút Quay lại
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có trang trước đó không, nếu có thì quay lại
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}