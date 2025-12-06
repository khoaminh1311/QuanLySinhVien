using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace InterfaceSinhVien
{
    public partial class RegisterCoursePage : Page
    {
        // Class hiển thị lên DataGrid
        public class LopHocItem
        {
            public string MaLop { get; set; }      // courseClass_id
            public string TenMon { get; set; }     // course name
            public string Phong { get; set; }      // room
            public string LichHoc { get; set; }    // learnSchedule
            public string TrangThai { get; set; }  // status (Còn chỗ/Đã đầy)
            public string MauTrangThai => TrangThai == "Đã đầy" ? "Red" : "Green";
        }

        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123;";
        string _studentID;

        public RegisterCoursePage(string studentID)
        {
            InitializeComponent();
            _studentID = studentID;
        }

        // --- 1. XỬ LÝ TÌM KIẾM ---
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập Mã môn hoặc Tên môn để tìm!");
                return;
            }

            List<LopHocItem> list = new List<LopHocItem>();

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // JOIN bảng CourseClass với Course để lấy tên môn
                    // Tìm kiếm gần đúng (LIKE) theo Mã môn hoặc Tên môn
                    string sql = @"
                        SELECT cc.courseClass_id, c.name, cc.room, cc.learnSchedule, cc.status
                        FROM CourseClass cc
                        JOIN Course c ON cc.course_id = c.course_id
                        WHERE cc.course_id LIKE @kw OR c.name LIKE @kw";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LopHocItem()
                            {
                                MaLop = reader["courseClass_id"].ToString(),
                                TenMon = reader["name"].ToString(),
                                Phong = reader["room"].ToString(),
                                LichHoc = reader["learnSchedule"].ToString(),
                                TrangThai = reader["status"].ToString()
                            });
                        }
                    }
                    dgLopHoc.ItemsSource = list;
                    if (list.Count == 0) MessageBox.Show("Không tìm thấy lớp học nào!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            }
        }

        // --- 2. XỬ LÝ ĐĂNG KÝ ---
        private void BtnDangKy_Click(object sender, RoutedEventArgs e)
        {
            string maLopMuonDangKy = ((Button)sender).Tag.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn đăng ký lớp {maLopMuonDangKy}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // --- BƯỚC 1: KIỂM TRA TRẠNG THÁI LỚP (MỚI THÊM) ---
                    string sqlCheckStatus = "SELECT status FROM CourseClass WHERE courseClass_id = @cid";
                    MySqlCommand cmdCheckStatus = new MySqlCommand(sqlCheckStatus, conn);
                    cmdCheckStatus.Parameters.AddWithValue("@cid", maLopMuonDangKy);

                    object resultStatus = cmdCheckStatus.ExecuteScalar();
                    string trangThaiLop = resultStatus != null ? resultStatus.ToString() : "";

                    // Kiểm tra chính xác chuỗi "Đã đầy" như trong ảnh database
                    if (trangThaiLop == "Đã đầy")
                    {
                        MessageBox.Show("Lớp này đã ĐẦY! Vui lòng chọn lớp khác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Dừng lại ngay, không cho đăng ký
                    }

                    // --- BƯỚC 2: KIỂM TRA SINH VIÊN ĐÃ ĐĂNG KÝ CHƯA ---
                    string checkSql = "SELECT COUNT(*) FROM Student_CourseClass WHERE student_id = @sid AND courseClass_id = @cid";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@sid", _studentID);
                    checkCmd.Parameters.AddWithValue("@cid", maLopMuonDangKy);

                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("Bạn đã đăng ký lớp này rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // --- BƯỚC 3: THỰC HIỆN ĐĂNG KÝ (INSERT) ---
                    string insertHocSql = "INSERT INTO Student_CourseClass (student_id, courseClass_id, quantity) VALUES (@sid, @cid, 1)";
                    MySqlCommand insertHocCmd = new MySqlCommand(insertHocSql, conn);
                    insertHocCmd.Parameters.AddWithValue("@sid", _studentID);
                    insertHocCmd.Parameters.AddWithValue("@cid", maLopMuonDangKy);
                    int rowsAffected = insertHocCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // --- BƯỚC 4: TỰ ĐỘNG THÊM VÀO LỊCH THI ---
                        // Tìm exam_id
                        string findExamSql = "SELECT exam_id FROM ExamSchedule WHERE courseClass_id = @cid";
                        MySqlCommand findExamCmd = new MySqlCommand(findExamSql, conn);
                        findExamCmd.Parameters.AddWithValue("@cid", maLopMuonDangKy);

                        List<string> listExamIDs = new List<string>();
                        using (MySqlDataReader reader = findExamCmd.ExecuteReader())
                        {
                            while (reader.Read()) listExamIDs.Add(reader["exam_id"].ToString());
                        }

                        // Tìm tên giáo viên để thêm vào Supervisor (nếu có)
                        string teacherName = "Chưa phân công";
                        string getTeacherSql = @"SELECT t.full_name FROM Teacher t JOIN Teacher_CourseClass tc ON t.teacher_id = tc.teacher_id WHERE tc.courseClass_id = @cid";
                        MySqlCommand teacherCmd = new MySqlCommand(getTeacherSql, conn);
                        teacherCmd.Parameters.AddWithValue("@cid", maLopMuonDangKy);
                        object resName = teacherCmd.ExecuteScalar();
                        if (resName != null) teacherName = resName.ToString();

                        int countExamAdded = 0;
                        foreach (string examID in listExamIDs)
                        {
                            string checkExamList = "SELECT COUNT(*) FROM ExamList WHERE exam_id = @eid AND student_id = @sid";
                            MySqlCommand cmdCheckEL = new MySqlCommand(checkExamList, conn);
                            cmdCheckEL.Parameters.AddWithValue("@eid", examID);
                            cmdCheckEL.Parameters.AddWithValue("@sid", _studentID);

                            if (Convert.ToInt32(cmdCheckEL.ExecuteScalar()) == 0)
                            {
                                string insertThiSql = "INSERT INTO ExamList (exam_id, student_id, status, supervisor_name) VALUES (@eid, @sid, 'Pending', @sup)";
                                MySqlCommand insertThiCmd = new MySqlCommand(insertThiSql, conn);
                                insertThiCmd.Parameters.AddWithValue("@eid", examID);
                                insertThiCmd.Parameters.AddWithValue("@sid", _studentID);
                                insertThiCmd.Parameters.AddWithValue("@sup", teacherName);
                                insertThiCmd.ExecuteNonQuery();
                                countExamAdded++;
                            }
                        }

                        MessageBox.Show($"Đăng ký thành công!\nGV: {teacherName}", "Thành công");
                    }
                    else
                    {
                        MessageBox.Show("Đăng ký thất bại. Vui lòng thử lại.", "Lỗi");
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