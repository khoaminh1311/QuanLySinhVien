using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace InterfaceSinhVien
{
    public class NotificationItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public bool IsNew => (DateTime.Now - Date).TotalDays < 3;
        public Visibility IsNewVisibility => IsNew ? Visibility.Visible : Visibility.Collapsed;

        public string TimeAgo
        {
            get
            {
                TimeSpan diff = DateTime.Now - Date;
                if (diff.TotalDays >= 1) return $"{(int)diff.TotalDays} ngày trước";
                if (diff.TotalHours >= 1) return $"{(int)diff.TotalHours} giờ {(int)diff.Minutes} phút trước";
                return "Vừa xong";
            }
        }
    }

    public partial class NewsPage : Page
    {
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123;";

        public NewsPage()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            List<NotificationItem> list = new List<NotificationItem>();

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // --- SỬA CÂU SQL: Bỏ id ---
                    string sql = "SELECT title, content, date FROM Notification " +
                                 "WHERE target = 'Sinh viên' OR target = 'Tất cả' " +
                                 "ORDER BY date DESC";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NotificationItem()
                            {
                                // --- CHỈ GÁN 3 CỘT NÀY ---
                                Title = reader["title"].ToString(),
                                Content = reader["content"].ToString(),
                                Date = Convert.ToDateTime(reader["date"])
                            });
                        }
                    }
                    lvThongBao.ItemsSource = list;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông báo: " + ex.Message);
                }
            }
        }
        private void LvThongBao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra dòng được chọn
            if (lvThongBao.SelectedItem is NotificationItem selectedItem)
            {
                // ĐIỀU HƯỚNG SANG PAGE CHI TIẾT
                // NavigationService tự động tìm Frame cha (ở MainWindow) để chuyển trang
                this.NavigationService.Navigate(new NotificationDetailPage(selectedItem));

                // Reset lại lựa chọn để lần sau bấm lại dòng đó vẫn nhận sự kiện
                lvThongBao.SelectedItem = null;
            }
        }
    }
}