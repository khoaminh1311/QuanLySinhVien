using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InterfaceSinhVien
{
    public partial class HomeworkPage : Page
    {
        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123;";
        string _studentID;

        // Class cho danh sách lớp (Cột trái)
        public class ClassItem
        {
            public string MaLop { get; set; } // courseClass_id
            public string TenMon { get; set; } // Course Name
        }

        // Class cho bài tập (Cột phải)
        public class HomeworkItem
        {
            public int Id { get; set; }
            public string Content { get; set; }

            // 1. Đổi sang DateTime (Không dùng string nữa)
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public DateTime? SubmitDate { get; set; }

            // Logic hiển thị trạng thái
            public string StatusText
            {
                get
                {
                    if (SubmitDate != null) return "✔ Đã nộp";

                    // 2. So sánh trực tiếp (Không cần Parse) -> Hết lỗi ngay lập tức
                    if (DateTime.Now > EndDate) return "⚠️ Quá hạn";

                    return "⏳ Đang làm";
                }
            }

            public SolidColorBrush StatusBgColor
            {
                get
                {
                    if (SubmitDate != null) return new SolidColorBrush(Color.FromRgb(220, 252, 231));

                    // 3. So sánh trực tiếp
                    if (DateTime.Now > EndDate) return new SolidColorBrush(Color.FromRgb(254, 226, 226));

                    return new SolidColorBrush(Color.FromRgb(254, 249, 195));
                }
            }

            public SolidColorBrush StatusForeColor
            {
                get
                {
                    if (SubmitDate != null) return new SolidColorBrush(Color.FromRgb(22, 163, 74));

                    // 4. So sánh trực tiếp
                    if (DateTime.Now > EndDate) return new SolidColorBrush(Color.FromRgb(220, 38, 38));

                    return new SolidColorBrush(Color.FromRgb(202, 138, 4));
                }
            }
        }

        public HomeworkPage(string studentID)
        {
            InitializeComponent();
            _studentID = studentID;
            LoadClasses();
        }

        // 1. TẢI DANH SÁCH LỚP
        private void LoadClasses()
        {
            List<ClassItem> listLop = new List<ClassItem>();
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    // Lấy mã lớp và tên môn từ bảng Student_CourseClass -> CourseClass -> Course
                    string sql = @"
                        SELECT scc.courseClass_id, c.name 
                        FROM Student_CourseClass scc
                        JOIN CourseClass cc ON scc.courseClass_id = cc.courseClass_id
                        JOIN Course c ON cc.course_id = c.course_id
                        WHERE scc.student_id = @sid";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sid", _studentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listLop.Add(new ClassItem()
                            {
                                MaLop = reader["courseClass_id"].ToString(),
                                TenMon = reader["name"].ToString()
                            });
                        }
                    }
                    lbClasses.ItemsSource = listLop;
                }
                catch (Exception ex) { MessageBox.Show("Lỗi tải lớp: " + ex.Message); }
            }
        }

        // 2. SỰ KIỆN CHỌN LỚP -> TẢI BÀI TẬP
        private void LbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbClasses.SelectedItem is ClassItem selectedClass)
            {
                txtEmpty.Visibility = Visibility.Collapsed;
                gridHomework.Visibility = Visibility.Visible;
                LoadHomework(selectedClass.MaLop);
            }
        }

        private void LoadHomework(string classID)
        {
            List<HomeworkItem> listHw = new List<HomeworkItem>();
            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // --- SỬA TÊN BẢNG TẠI ĐÂY (Homework -> Assignment) ---
                    string sql = "SELECT id, content, start_date, end_date, submit_date FROM Assignment WHERE courseClass_id = @cid";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cid", classID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listHw.Add(new HomeworkItem()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Content = reader["content"].ToString(),
                                StartDate = Convert.ToDateTime(reader["start_date"]),
                                EndDate = Convert.ToDateTime(reader["end_date"]),

                                // Kiểm tra null cho ngày nộp (vì có thể chưa nộp)
                                SubmitDate = reader["submit_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["submit_date"]) : null
                            });
                        }
                    }
                    dgHomework.ItemsSource = listHw;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải bài tập: " + ex.Message);
                }
            }

        }
        private void DgHomework_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgHomework.SelectedItem is HomeworkItem selectedHw)
            {
                // THAY ĐỔI TẠI ĐÂY: Điều hướng sang Page thay vì mở Window
                this.NavigationService.Navigate(new HomeworkDetailPage(selectedHw));
            }
        }
    }
}