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
    /// Interaction logic for AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        public AddTeacherWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text.Trim();
            string name = txtName.Text.Trim();
            string gender = (cbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string faculty = txtFaculty.Text.Trim();

            // Kiểm tra nhập đầy đủ
            if (id == "" || name == "" || gender == "" || email == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // ✓ Kiểm tra teacher_id phải đúng 4 ký tự
            if (id.Length != 4)
            {
                MessageBox.Show("Mã giảng viên phải gồm đúng 4 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO teacher(teacher_id, full_name, gender, email, phone, address, faculty)
                       VALUES(@id,@name,@gender,@email,@phone,@address,@faculty)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@faculty", faculty);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm giảng viên thành công!");
            this.Close();
        }


    }
}
