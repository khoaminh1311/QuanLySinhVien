using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace InterfaceSinhVien
{
    public partial class DangNhap : Window
    {
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123456;";

        public DangNhap()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text; // Đây chính là student_id người dùng nhập
            string pass = txtPassword.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Kiểm tra đúng ID và Pass chưa
                    string sql = "SELECT * FROM Student WHERE student_id = @user AND password = @pass";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Đăng nhập thành công -> Lấy thông tin cần thiết
                            string id = reader["student_id"].ToString();
                            string name = reader["full_name"].ToString();

                            MessageBox.Show("Xin chào: " + name);

                            // MỞ TRANG CHỦ VÀ TRUYỀN DỮ LIỆU SANG
                            MainWindow main = new MainWindow(id, name);
                            main.Show();

                            this.Close(); // Đóng form đăng nhập
                        }
                        else
                        {
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message);
                }
            }
        }

        // Hàm này giữ nguyên (trống cũng được)
        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
    }
}