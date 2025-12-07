using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Management_system
{
    /// <summary>
    /// Interaction logic for AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        public AddStudentWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text.Trim();
            string name = txtName.Text.Trim();
            string gender = txtGender.Text.Trim();
            string email = txtEmail.Text.Trim();
            string major = txtMajor.Text.Trim();

            // --- KIỂM TRA MÃ SV PHẢI 4 KÝ TỰ ---
            if (id.Length != 4)
            {
                MessageBox.Show("Mã sinh viên phải gồm đúng 4 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- TỰ ĐỘNG LẤY KHÓA LÀ KÝ TỰ ĐẦU CỦA MSSV ---
            string khoa = id.Substring(0, 1); // ký tự đầu tiên

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO student 
                        (student_id, full_name, gender, email, khoa, major)
                        VALUES (@id, @name, @gender, @email, @khoa, @major)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@khoa", khoa);   // TỰ ĐỘNG LẤY
                cmd.Parameters.AddWithValue("@major", major);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm sinh viên thành công!");
            this.Close();
        }

    }
}
