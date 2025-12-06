using System.Windows.Controls; // Dùng cho Page
using System.Windows;
using MySql.Data.MySqlClient;
using System;

namespace InterfaceSinhVien
{
    // Sửa class thành Page
    public partial class UserProfilePage : Page
    {
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123;";
        string _currentStudentID;

        public UserProfilePage(string studentID)
        {
            InitializeComponent();
            _currentStudentID = studentID;
            LayThongTinChiTiet();
        }

        private void LayThongTinChiTiet()
        {
            // (Code y hệt bài trước, không thay đổi gì cả)
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT * FROM Student WHERE student_id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _currentStudentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtMaSV.Text = reader["student_id"].ToString();
                            txtHoTen.Text = reader["full_name"].ToString();
                            txtGioiTinh.Text = reader["gender"].ToString();
                            txtEmail.Text = reader["email"].ToString();
                            txtPhone.Text = reader["phone"].ToString();
                            txtDiaChi.Text = reader["address"].ToString();
                            txtNganh.Text = reader["major"].ToString();
                            txtKhoa.Text = reader["khoa"].ToString();
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }
}