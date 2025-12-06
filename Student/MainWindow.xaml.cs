using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuanLySinhVien.View;

namespace InterfaceSinhVien
{
    public partial class MainWindow : Window
    {
        private string _studentID;
        private string _studentName;

        // Biến lưu nút đang được chọn hiện tại
        private Button _currentActiveButton;

        public MainWindow(string id, string hoTen)
        {
            InitializeComponent();
            _studentID = id;
            _studentName = hoTen;

            txtTenHeader.Text = " " + hoTen;
            txtTenPanel.Text = hoTen;

            // Mặc định chọn nút Thông báo đầu tiên
            SetActiveMenu(btnThongBao);
            MainFrame.Navigate(new UserProfilePage(_studentID));
        }

        // --- HÀM XỬ LÝ MÀU SẮC (LOGIC CHUẨN) ---
        private void SetActiveMenu(object sender)
        {
            Button clickedButton = sender as Button;

            // Nếu không lấy được nút (hoặc click ra ngoài), thì không làm gì cả
            if (clickedButton == null) return;

            // 1. Reset nút cũ (Nếu có) về trạng thái bình thường
            if (_currentActiveButton != null)
            {
                _currentActiveButton.Background = Brushes.Transparent;
                // Trả về màu chữ xám đậm (#334155) cho nút cũ
                _currentActiveButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155"));
            }

            // 2. Tô màu cho nút MỚI
            // Màu đỏ (#D32F2F)
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
            clickedButton.Foreground = Brushes.White;

            // 3. Cập nhật biến _currentActiveButton thành nút mới này
            _currentActiveButton = clickedButton;
        }

        // --- CÁC SỰ KIỆN CLICK (ĐÃ GỌI HÀM SetActiveMenu) ---

        private void BtnThongTin_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender); // Gửi chính nút này vào hàm xử lý
            MainFrame.Navigate(new UserProfilePage(_studentID));
        }

        private void BtnTinTuc_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender);
            MainFrame.Navigate(new NewsPage());
        }

        private void BtnLich_Tong_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender);

            if (MenuLichCon.Visibility == Visibility.Visible)
                MenuLichCon.Visibility = Visibility.Collapsed;
            else
                MenuLichCon.Visibility = Visibility.Visible;
        }

        private void BtnLichHoc_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(btnLich);

            MainFrame.Navigate(new SchedulePage(_studentID, 0));
        }

        private void BtnLichThi_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(btnLich); // Vẫn giữ đỏ nút cha
            MainFrame.Navigate(new SchedulePage(_studentID, 1));
        }

        private void BtnXemDiem_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender);
            MainFrame.Navigate(new GradePage(_studentID));
        }

        private void BtnDangKyMon_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender);
            MainFrame.Navigate(new RegisterCoursePage(_studentID));
        }

        private void BtnHocPhi_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode(false);
            SetActiveMenu(sender);
            MainFrame.Navigate(new TuitionPage(_studentID, _studentName));
        }
        private void SetFullScreenMode(bool isFull)
        {
            if (isFull)
            {
                // CHẾ ĐỘ TOÀN MÀN HÌNH (Ẩn Sidebar)
                SidebarBorder.Visibility = Visibility.Collapsed;

                // Chỉnh Frame tràn ra 2 cột (Cột 0 và Cột 1)
                Grid.SetColumn(MainFrame, 0);
                Grid.SetColumnSpan(MainFrame, 2);

                // Bỏ bo góc và margin để nhìn liền mạch hơn
                MainFrame.Margin = new Thickness(0);
            }
            else
            {
                // CHẾ ĐỘ BÌNH THƯỜNG (Hiện Sidebar)
                SidebarBorder.Visibility = Visibility.Visible;

                // Trả Frame về cột 1
                Grid.SetColumn(MainFrame, 1);
                Grid.SetColumnSpan(MainFrame, 1);

                // Trả lại style cũ
                MainFrame.Margin = new Thickness(5);
            }
        }

        // 2. Sửa sự kiện nút "Bài Tập" (Trên Header) -> Vào chế độ Full
        private void BtnBaiTap_Click(object sender, RoutedEventArgs e)
        {
            // Bật chế độ toàn màn hình
            SetFullScreenMode(true);

            // Reset nút active ở menu trái (vì menu bị ẩn rồi)
            SetActiveMenu(null);

            // Điều hướng vào trang Bài tập
            MainFrame.Navigate(new HomeworkPage(_studentID));
        }
        private void BtnTrangChu_Click(object sender, RoutedEventArgs e)
        {
            // Tắt chế độ toàn màn hình (Hiện lại Sidebar)
            SetFullScreenMode(false);

            // Active lại nút mặc định (Thông tin cá nhân)
            SetActiveMenu(btnThongBao);

            // Điều hướng về trang Profile (hoặc trang nào bạn muốn làm trang chủ)
            MainFrame.Navigate(new UserProfilePage(_studentID));
        }
        private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            // Thoát chế độ toàn màn hình (nếu đang ở trang Bài tập)
            SetFullScreenMode(false);

            // Tắt highlight menu trái
            SetActiveMenu(null);

            // Điều hướng sang trang Báo Cáo
            MainFrame.Navigate(new ReportPage(_studentID));
        }
        private void Button_Thoat(object sender, RoutedEventArgs e)
        {
           Login lg = new Login();
           lg.Show();
           this.Close();
        }
    }
}