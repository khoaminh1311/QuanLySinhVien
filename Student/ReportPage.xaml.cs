using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InterfaceSinhVien
{
    public partial class ReportPage : Page
    {
        // Nhớ thêm SslMode=None
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123456;";

        // Dù ẩn danh, ta vẫn giữ constructor nhận ID để code ở MainWindow không bị lỗi
        // Nhưng ta sẽ KHÔNG dùng biến _studentID này khi Insert
        public ReportPage(string studentID)
        {
            InitializeComponent();
        }

        private void BtnGuiBaoCao_Click(object sender, RoutedEventArgs e)
        {
            string content = txtContent.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung báo cáo!", "Thông báo");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // --- SỬA LẠI: GỬI ẨN DANH (Bỏ student_id) ---
                    // Chỉ insert Nội dung và Thời gian
                    string sql = "INSERT INTO Report (content, request_date) VALUES (@content, NOW())";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@content", content);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Gửi báo cáo ẩn danh thành công! Cảm ơn đóng góp của bạn.", "Thành công");
                        txtContent.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }
        }
    }
}