using MySql.Data.MySqlClient;
using System;
using System.Diagnostics; // Dùng để mở trình duyệt
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InterfaceSinhVien
{
    public partial class TuitionPage : Page
    {
        // Nhớ thêm SslMode=None để tránh lỗi "Reading from stream failed"
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123;";
        string _studentID;
        string _fullName;

        public TuitionPage(string studentID, string fullName)
        {
            InitializeComponent();
            _studentID = studentID;
            _fullName = fullName;

            txtNoiDungCK.Text = $"{_fullName} {_studentID} HOC PHI HK1 2025";
            LoadThongTinHocPhi();
        }

        private void LoadThongTinHocPhi()
        {
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // 1. LẤY HỌC PHÍ & TRẠNG THÁI TỪ BẢNG STUDENT
                    string sqlStudent = "SELECT a.tuition, b.tuitionStatus FROM Tuition_Processing as b Join Student as a on a.student_id = b.student_id WHERE b.student_id = @sid";
                    MySqlCommand cmdStudent = new MySqlCommand(sqlStudent, conn);
                    cmdStudent.Parameters.AddWithValue("@sid", _studentID);

                    using (MySqlDataReader reader = cmdStudent.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal tuition = reader["tuition"] != DBNull.Value ? Convert.ToDecimal(reader["tuition"]) : 0;
                            txtTongTien.Text = string.Format("{0:N0} VNĐ", tuition);

                            string status = reader["tuitionStatus"] != DBNull.Value ? reader["tuitionStatus"].ToString() : "Chưa nộp";
                            UpdateStatusUI(status);
                        }
                    }

                    // 2. LẤY LINK BIÊN LAI TỪ TUITION_PROCESSING (Sửa đoạn này)
                    string sqlLink = "SELECT bill_url FROM Tuition_Processing WHERE student_id = @sid";
                    MySqlCommand cmdLink = new MySqlCommand(sqlLink, conn);
                    cmdLink.Parameters.AddWithValue("@sid", _studentID);

                    object linkObj = cmdLink.ExecuteScalar();
                    if (linkObj != null)
                    {
                        // Điền link vào ô TextBox
                        txtLinkBienLai.Text = linkObj.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void UpdateStatusUI(string status)
        {
            txtTrangThai.Text = status;

            if (status == "Đã nộp")
            {
                borderStatus.Background = new SolidColorBrush(Colors.LightGreen);
                txtTrangThai.Foreground = new SolidColorBrush(Colors.Green);

                // Khoá ô nhập liệu nếu đã xong
                txtLinkBienLai.IsEnabled = false;
                btnGuiYeuCau.IsEnabled = false;
                btnGuiYeuCau.Content = "✔ Đã hoàn thành";
                btnGuiYeuCau.Background = new SolidColorBrush(Colors.Gray);
            }
            else if (status == "Chờ xét duyệt")
            {
                borderStatus.Background = new SolidColorBrush(Colors.Orange);
                txtTrangThai.Foreground = new SolidColorBrush(Colors.White);
                btnGuiYeuCau.Content = "Cập nhật Link Mới"; // Cho phép sửa link
            }
            else if (status == "Không hợp lệ")
            {
                borderStatus.Background = new SolidColorBrush(Colors.Red);
                txtTrangThai.Foreground = new SolidColorBrush(Colors.White);
            }
            else // Chưa nộp
            {
                borderStatus.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        // --- SỰ KIỆN MỞ LINK TRÌNH DUYỆT ---
        private void BtnMoLink_Click(object sender, RoutedEventArgs e)
        {
            string url = txtLinkBienLai.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    // Lệnh mở trình duyệt mặc định
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Link không hợp lệ hoặc không thể mở trình duyệt.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập link trước.");
            }
        }

        private void BtnCopySTK_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("1234567889");
            MessageBox.Show("Đã sao chép số tài khoản!");
        }

        // --- SỰ KIỆN GỬI YÊU CẦU (LƯU LINK) ---
        private void BtnGuiYeuCau_Click(object sender, RoutedEventArgs e)
        {
            string linkUrl = txtLinkBienLai.Text.Trim();

            // Kiểm tra link rỗng
            if (string.IsNullOrEmpty(linkUrl))
            {
                MessageBox.Show("Vui lòng dán Link Google Drive hoặc link ảnh vào!", "Thông báo");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // BƯỚC A: Lưu LINK vào bảng Tuition_Processing
                    string checkSql = "SELECT COUNT(*) FROM Tuition_Processing WHERE student_id = @sid";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@sid", _studentID);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    string sqlProcess = "";
                    if (count > 0)
                        sqlProcess = "UPDATE Tuition_Processing SET bill_url = @url, request_date = NOW(), tuitionStatus = 'Chờ xét duyệt' WHERE student_id = @sid";
                    else
                        sqlProcess = "INSERT INTO Tuition_Processing (student_id, bill_url, request_date, tuitionStatus) VALUES (@sid, @url, NOW(), 'Chờ xét duyệt')";

                    MySqlCommand cmdProcess = new MySqlCommand(sqlProcess, conn);
                    cmdProcess.Parameters.AddWithValue("@sid", _studentID);
                    cmdProcess.Parameters.AddWithValue("@url", linkUrl); // Lưu Link
                    cmdProcess.ExecuteNonQuery();

                    // BƯỚC B: Cập nhật trạng thái Student
                    string sqlStudent = "UPDATE Student SET tuitionStatus = 'Chờ xét duyệt' WHERE student_id = @sid";
                    MySqlCommand cmdStudent = new MySqlCommand(sqlStudent, conn);
                    cmdStudent.Parameters.AddWithValue("@sid", _studentID);
                    cmdStudent.ExecuteNonQuery();

                    MessageBox.Show("Gửi link thành công!.", "Thành công");

                    LoadThongTinHocPhi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}