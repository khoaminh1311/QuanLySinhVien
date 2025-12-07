using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM admin LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

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

                    string avatar = reader["avatar"]?.ToString();
                    LoadAvatar(avatar);
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
