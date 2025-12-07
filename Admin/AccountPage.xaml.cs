using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QuanLySinhVien.Utils;
namespace Management_system.Pages
{
    public partial class AccountPage : Page
    {
        string currentAdminId;

        public AccountPage()
        {
            InitializeComponent();
            LoadAdminInfo();
        }

        public static string ConvertGoogleDriveToDirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            var match1 = System.Text.RegularExpressions.Regex.Match(url,
                @"\/file\/d\/([a-zA-Z0-9_-]+)");

            if (match1.Success)
                return $"https://drive.google.com/uc?export=view&id={match1.Groups[1].Value}";

            var match2 = System.Text.RegularExpressions.Regex.Match(url,
                @"id=([a-zA-Z0-9_-]+)");

            if (match2.Success)
                return $"https://drive.google.com/uc?export=view&id={match2.Groups[1].Value}";

            return url;
        }

        private void LoadAdminInfo()
        {
            // 1. Lấy từ App trước (nếu có)
            string adminId = App.AdminID;

            // 2. Nếu rỗng, lấy từ class Admin trong Management_system (nếu có dùng)
            if (string.IsNullOrWhiteSpace(adminId))
            {
                adminId = Admin.AdminId;
            }

            // 3. Nếu vẫn rỗng, lấy từ UserSession (được set ở màn Login chính)
            if (string.IsNullOrWhiteSpace(adminId))
            {
                adminId = UserSession.UserId;
            }

            // 4. Nếu vẫn không có thì chịu, báo lỗi
            if (string.IsNullOrWhiteSpace(adminId))
            {
                MessageBox.Show(
                    "Không xác định được mã admin đang đăng nhập (App.AdminID, Admin.AdminId, UserSession.UserId đều rỗng).");
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM admin WHERE admin_id = @id LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = adminId;

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    currentAdminId = reader["admin_id"].ToString();

                    DataContext = new
                    {
                        AdminId = reader["admin_id"].ToString(),
                        AdminName = reader["admin_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Email = reader["email"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Address = reader["address"].ToString(),
                    };

                    LoadAvatar(reader["avatar"]?.ToString());
                }
                else
                {
                    MessageBox.Show("Không tìm thấy admin_id = " + adminId);
                }
            }
        }





        private void LoadAvatar(string avatar)
        {
            try
            {
                if (string.IsNullOrEmpty(avatar))
                {
                    SetDefaultAvatar();
                    return;
                }

                // Nếu trong DB chưa có đuôi file, thêm mặc định .png
                if (!avatar.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !avatar.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                    !avatar.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    avatar += ".png";
                }

                // Đường dẫn ảnh local: Admin/images/<avatar>
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string localPath = Path.Combine(baseDir, "Admin", "images", avatar);

                if (File.Exists(localPath))
                {
                    imgAvatar.Fill = new ImageBrush(
                        new BitmapImage(new Uri(localPath, UriKind.Absolute))
                    );
                    return;
                }

                // Google Drive
                if (avatar.Contains("drive.google.com"))
                {
                    string url = ConvertGoogleDriveToDirect(avatar);

                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(url, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();

                    imgAvatar.Fill = new ImageBrush(bmp);
                    return;
                }

                // Link http bình thường
                imgAvatar.Fill = new ImageBrush(new BitmapImage(new Uri(avatar)));
            }
            catch
            {
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string avatarPath = Path.Combine(baseDir, "Admin", "images", "admin_avatar.png");

            if (File.Exists(avatarPath))
            {
                imgAvatar.Fill = new ImageBrush(
                    new BitmapImage(new Uri(avatarPath, UriKind.Absolute))
                );
            }
            else
            {
                imgAvatar.Fill = new SolidColorBrush(Colors.LightGray);
            }
        }




        private void EditInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditAdminWindow edit = new EditAdminWindow(currentAdminId);
            edit.ShowDialog();
            LoadAdminInfo();  // load lại sau khi sửa
        }
    }
}
