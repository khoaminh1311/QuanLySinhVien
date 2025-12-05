using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace InterfaceSinhVien
{
    public class NotificationItem
    {
        public int Id { get; set; }
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
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123456;";

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

                    // --- CẬP NHẬT CÂU SQL TẠI ĐÂY ---
                    // Thêm điều kiện: WHERE target = 'Sinh viên'
                    // Mẹo: Nên thêm cả OR target = 'Tất cả' để sinh viên không bị sót các thông báo chung

                    string sql = "SELECT noti_id, title, content, date FROM Notification " +
                                 "WHERE target = 'Sinh viên' OR target = 'Tất cả' " +
                                 "ORDER BY date DESC";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NotificationItem()
                            {
                                Id = Convert.ToInt32(reader["noti_id"]),
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
                    MessageBox.Show("Lỗi: " + ex.Message);
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