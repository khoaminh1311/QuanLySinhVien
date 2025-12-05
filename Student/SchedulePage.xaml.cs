using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient; // Nhớ cài thư viện này

namespace InterfaceSinhVien
{
    public partial class SchedulePage : Page
    {
        // Class dùng cho Tab Lịch Học
        public class LichHocItem
        {
            public string MaLop { get; set; }      // courseClass_id
            public string TenMon { get; set; }     // course_id (VD: CS101)
            public string PhongHoc { get; set; }   // room
            public string ThoiGian { get; set; }   // learnSchedule
            public string ThoiLuong { get; set; }  // duration
        }

        // Class dùng cho Tab Lịch Thi
        public class LichThiItem
        {
            public string MaKyThi { get; set; }    // exam_id
            public string MaLop { get; set; }      // courseClass_id
            public string NgayGio { get; set; }    // schedule (Ngày giờ thi)
            public string PhongThi { get; set; }   // room (Lấy từ bảng ExamList hoặc ExamSchedule)
            public string GiamThi { get; set; }    // supervisor_name
            public string TrangThai { get; set; }  // status
        }
        // CẤU HÌNH KẾT NỐI (Sửa lại cho đúng máy bạn)
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123456;";
        string _studentID;

        public SchedulePage(string studentID, int initialTabIndex = 0)
        {
            InitializeComponent();
            _studentID = studentID;

            LoadLichHoc();
            LoadLichThi();

            // Tự động chuyển tab dựa vào nút người dùng bấm ở menu
            if (initialTabIndex >= 0 && initialTabIndex < TabControlLich.Items.Count)
            {
                TabControlLich.SelectedIndex = initialTabIndex;
            }
        }
        // ==========================================
        // 1. TẢI LỊCH HỌC
        // Logic: Student_CourseClass -> CourseClass
        // ==========================================
        private void LoadLichHoc()
        {
            List<LichHocItem> list = new List<LichHocItem>();
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Query dựa trên ảnh image_556652.png và image_556998.png
                    string sql = @"
                        SELECT 
                            cc.courseClass_id, 
                            cc.course_id, 
                            cc.room, 
                            cc.learnSchedule, 
                            cc.duration
                        FROM Student_CourseClass scc
                        JOIN CourseClass cc ON scc.courseClass_id = cc.courseClass_id
                        WHERE scc.student_id = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _studentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LichHocItem()
                            {
                                MaLop = reader["courseClass_id"].ToString(),
                                TenMon = reader["course_id"].ToString(),
                                PhongHoc = reader["room"].ToString(),
                                ThoiGian = reader["learnSchedule"].ToString(),
                                ThoiLuong = reader["duration"].ToString()
                            });
                        }
                    }
                    dgLichHoc.ItemsSource = list; // Đổ dữ liệu lên DataGrid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải lịch học: " + ex.Message);
                }
            }
        }

        // ==========================================
        // 2. TẢI LỊCH THI
        // Logic: ExamList (tìm sv) -> JOIN ExamSchedule (lấy giờ)
        // ==========================================
        private void LoadLichThi()
        {
            List<LichThiItem> list = new List<LichThiItem>();
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // Query dựa trên ảnh image_55630d.png (ExamList) và image_5565da.png (ExamSchedule)
                    // Ta lấy ExamList làm gốc vì nó chứa sinh viên cụ thể
                    string sql = @"
                        SELECT 
                            el.exam_id,
                            es.courseClass_id,
                            es.schedule,
                            el.supervisor_name,
                            el.status
                        FROM ExamList el
                        JOIN ExamSchedule es ON el.exam_id = es.exam_id
                        WHERE el.student_id = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _studentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LichThiItem()
                            {
                                MaKyThi = reader["exam_id"].ToString(),
                                MaLop = reader["courseClass_id"].ToString(),
                                NgayGio = reader["schedule"].ToString(),
                                GiamThi = reader["supervisor_name"].ToString(),
                                TrangThai = reader["status"].ToString()
                            });
                        }
                    }
                    dgLichThi.ItemsSource = list;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải lịch thi: " + ex.Message);
                }
            }
        }
    }
}