using System.Windows;
using AdminPortal;
using InterfaceSinhVien;
using QuanLySinhVien.Repositories;
using QuanLySinhVien.Utils;
using QuanLySinhVien.Views.Teacher;

namespace QuanLySinhVien.View
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private string GetRole()
        {
            if (rtn_student.IsChecked == true) return "student";
            if (rtn_teacher.IsChecked == true) return "teacher";
            if (rtn_admin.IsChecked == true) return "admin";

            return string.Empty;
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Password.Trim();
            string role = GetRole();

            if (role == "")
            {
                MessageBox.Show("Vui lòng chọn vai trò đăng nhập!");
                return;
            }

            var repo = new LoginRepository();
            bool ok = repo.CheckLogin(user, pass, role);

            if (!ok)
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!");
                return;
            }

            // LƯU SESSION
            UserSession.UserId = user;
            UserSession.Role = role;

            // MỞ DASHBOARD TƯƠNG ỨNG
            string name = repo.GetName(user, role);
            UserSession.FullName = name;
            UserSession.UserId = user;
            UserSession.Role = role;
            if(role == "teacher")
            {
                var dash = new TeacherDashboard(UserSession.UserId, UserSession.FullName);
                dash.Show();
                this.Close();
            } else if(role == "student")
            {
                var dash = new InterfaceSinhVien.MainWindow(UserSession.UserId, UserSession.FullName);
                dash.Show();
                this.Close();
            } else if(role == "admin")
            {
                var dash = new AdminPortal.MainWindow();
                dash.Show();
                this.Close();
            }
        }
    }
}
