using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace InterfaceSinhVien
{
    public partial class GradePage : Page
    {
        public class DiemItem
        {
            public string TenMon { get; set; }
            public int TinChi { get; set; }
            public double ChuyenCan { get; set; }
            public double GiuaKy { get; set; }
            public double CuoiKy { get; set; }
            public double TongKet { get; set; }
        }

        string strKetNoi = "Server=127.0.0.1;Database=doanqlsv;Port=3307;Uid=hung;Pwd=123456;";
        string _studentID;

        public GradePage(string studentID)
        {
            InitializeComponent();
            _studentID = studentID;
            LoadDiem();
        }

        private void LoadDiem()
        {
            List<DiemItem> list = new List<DiemItem>();

            using (MySqlConnection conn = new MySqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            c.name AS TenMon,
                            c.credit AS TinChi,
                            g.attendance,
                            g.mid_term,
                            g.final_term,
                            g.total
                        FROM Grade g
                        JOIN CourseClass cc ON g.courseClass_id = cc.courseClass_id
                        JOIN Course c ON cc.course_id = c.course_id
                        WHERE g.student_id = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _studentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DiemItem()
                            {   
                                TenMon = reader["TenMon"].ToString(),
                                TinChi = Convert.ToInt32(reader["TinChi"]),

                                // --- SỬA Ở ĐÂY: Dùng Math.Round(..., 1) để lấy 1 số lẻ ---
                                ChuyenCan = Math.Round(Convert.ToDouble(reader["attendance"]), 1),
                                GiuaKy = Math.Round(Convert.ToDouble(reader["mid_term"]), 1),
                                CuoiKy = Math.Round(Convert.ToDouble(reader["final_term"]), 1),
                                TongKet = Math.Round(Convert.ToDouble(reader["total"]), 1)
                            });
                        }
                    }
                    dgDiem.ItemsSource = list;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải bảng điểm: " + ex.Message);
                }
            }
        }
    }
}