using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MySqlX.XDevAPI;
using QuanLySinhVien.Models;
using QuanLySinhVien.Repositories;
using QuanLySinhVien.Utils;
using QuanLySinhVien.View;


namespace QuanLySinhVien.Views.Teacher
{
    public partial class TeacherDashboard : Window
    {
        private TeacherInfoModel currentTeacher;
        private List<CourseClassModel> fullClassList;
        private readonly AdvisorClassRepository advisorRepo = new AdvisorClassRepository();
        private NotificationService notiService = new NotificationService();
        private TeachingScheduleRepository _courseRepo = new TeachingScheduleRepository();

        //cloudflared access tcp --hostname mysql.doanqlsv.id.vn --url localhost:3307

        private readonly string _teacherId;
        private readonly string _teacherName;

        public TeacherDashboard(string teacherId, string teacherName)
        {
            InitializeComponent();
            //dataGridDiem.AutoGenerateColumns = false;
            Loaded += TeacherDashboard_Loaded;

            // GÁN GIÁ TRỊ SESSION
            UserSession.UserId = teacherId;
            UserSession.FullName = teacherName;

            txtTenGV_Header.Text = $"Giảng viên: {teacherName}";
            txtTenGV_Left.Text = teacherName;

            // NẠP THÔNG TIN GIÁO VIÊN
            var repo = new TeacherProfileRepository();
            currentTeacher = repo.GetTeacherProfile(teacherId);

            LoadSemesters();
            cbSemester.SelectionChanged += cbSemester_SelectionChanged;
            cbClass.SelectionChanged += cbClass_SelectionChanged;
        }

        private CourseClassRepository courseRepo = new CourseClassRepository();

        private void LoadSemesters()
        {
            cbSemester.ItemsSource = courseRepo.GetSemesters();
            cbSemester.SelectedIndex = 0;
        }
        private async void TeacherDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            txtTenGV_Header.Text = "Giảng viên: " + currentTeacher.FullName;
            txtTenGV_Left.Text = currentTeacher.FullName;
            //txtTitle.Text = "THÔNG TIN CÁ NHÂN";
            var repo = new TeacherProfileRepository();
            currentTeacher = repo.GetTeacherProfile(UserSession.UserId);

            // AVATAR — ƯU TIÊN ẢNH VỪA ĐỔI (UserSession)
            if (!string.IsNullOrEmpty(UserSession.AvatarPath))
            {
                LoadAvatar(UserSession.AvatarPath);
            }
            // Nếu không có ảnh mới, dùng ảnh từ database
            else if (!string.IsNullOrEmpty(currentTeacher.AvatarPath))
            {
                LoadAvatar(currentTeacher.AvatarPath);
            }

            var repo1 = new TeacherRepository();
            repo1.UpdateAvatar(UserSession.UserId, UserSession.AvatarPath);

            var classRepo = new CourseClassRepository();
            var teacherClasses = classRepo.GetCourseClassesByTeacher(UserSession.UserId);

            cbGiaoBT_Class.ItemsSource = teacherClasses;
            cbGiaoBT_Class.DisplayMemberPath = "CourseClassId";


            ShowOnly(gridNoiQuy);

            // HIỂN THỊ PROFILE RA GIAO DIỆN
            txtProfileFullName.Text = currentTeacher.FullName;
            txtProfileTeacherId.Text = currentTeacher.TeacherId;
            txtProfileGender.Text = currentTeacher.Gender;
            txtProfileEmail.Text = currentTeacher.Email;
            txtProfilePhone.Text = currentTeacher.Phone;
            txtProfileAddress.Text = currentTeacher.Address;
            txtProfileFaculty.Text = currentTeacher.Faculty;
            txtTenGV_Header.Text = "Giảng viên: " + currentTeacher.FullName;
            txtTenGV_Left.Text = currentTeacher.FullName;
            //txtTitle.Text = "THÔNG TIN CÁ NHÂN";

        }

        private void SetActive(object sender)
        {

            // Đặt active mới
            (sender as Border).Tag = "Active";
        }

        // Các sự kiện click
        private void OpenProfile(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
            // TODO: mở profile
        }

        private void OpenLichDay(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
        }

        private void OpenDSLopDay(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
        }

        private void OpenQuanLiDiem(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
        }

        private void OpenCoVan(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
        }

        private void OpenCoiThi(object sender, MouseButtonEventArgs e)
        {
            SetActive(sender);
        }

        private void ShowOnly(Grid target)
        {
            gridNoiQuy.Visibility = Visibility.Collapsed;
            gridProfile.Visibility = Visibility.Collapsed;
            gridLichDay.Visibility = Visibility.Collapsed;
            gridDanhSachLopDay.Visibility = Visibility.Collapsed;
            gridQuanLyDiem.Visibility = Visibility.Collapsed;
            gridLopCoVan.Visibility = Visibility.Collapsed;
            gridLichCoiThi.Visibility = Visibility.Collapsed;
            gridNotification.Visibility = Visibility.Collapsed;
            gridGiaoBaiTap.Visibility = Visibility.Collapsed;
            gridQuanLyAssignment.Visibility = Visibility.Collapsed;
            gridXemNopBai.Visibility = Visibility.Collapsed;

            target.Visibility = Visibility.Visible;
        }
        private void XemProfile_Click(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "THÔNG TIN CÁ NHÂN";
            ShowOnly(gridProfile);
            LoadTeacherProfile();
        }

        private void LoadTeacherProfile()
        {
            var repo = new TeacherProfileRepository();
            var info = repo.GetTeacherProfile(UserSession.UserId);

            txtProfileFullName.Text = info.FullName;
            txtProfileTeacherId.Text = info.TeacherId;
            txtProfileGender.Text = info.Gender;
            txtProfileEmail.Text = info.Email;
            txtProfilePhone.Text = info.Phone;
            txtProfileAddress.Text = info.Address;
            txtProfileFaculty.Text = info.Faculty;
        }


        private void XemLichDay_Click(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "LỊCH DẠY TUẦN NÀY";
            ShowOnly(gridLichDay);

            // 1) Load ItemsSource trước
            cbx_hockilichday.ItemsSource = _courseRepo.GetSemesters();
            cbx_namlichday.ItemsSource = _courseRepo.GetYears();

            // 2) Chọn item mặc định
            cbx_hockilichday.SelectedIndex = 0;
            cbx_namlichday.SelectedIndex = 0;

            string semester = cbx_hockilichday.SelectedItem.ToString();
            string year = cbx_namlichday.SelectedItem.ToString();

            var repo = new TeachingScheduleRepository();
            var data = repo.GetTeachingSchedule(UserSession.UserId, semester, year);

            dataGridLichDay.ItemsSource = data;
            dataGridLichDay.Visibility = Visibility.Visible;
        }

        private void XemDanhSachLopDay_Click(object sender, MouseButtonEventArgs e)
        {
            ShowOnly(gridDanhSachLopDay);
            //gridDanhSachLopDay.Visibility = Visibility.Collapsed;
            txtTitle.Text = "DANH SÁCH LỚP ĐANG GIẢNG DẠY";

            var repo = new CourseClassRepository();
            fullClassList = repo.GetCourseClassesByTeacher(UserSession.UserId);
            dataGridLopDay.ItemsSource = fullClassList;
            dataGridLopDay.Visibility = Visibility.Visible;

            cbx_hockilopday.ItemsSource = _courseRepo.GetSemesters();
            cbx_namlopday.ItemsSource = _courseRepo.GetYears();
        }

        private void QuanLyDiem_Click(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "QUẢN LÝ ĐIỂM";
            ShowOnly(gridQuanLyDiem);
            gridQuanLyDiem.Visibility = Visibility.Visible;

            // Lấy lớp đang chọn
            string selectedClassId = "CS101-A";
            var repo = new GradeRepository();
            var list = repo.GetStudentGrades(selectedClassId, UserSession.UserId);

            dataGridDiem.ItemsSource = list;
        }

        private void XemDanhSachLopCoVan_Click(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "DANH SÁCH LỚP CỐ VẤN";
            ShowOnly(gridLopCoVan);

            string teacherId = UserSession.UserId;

            var list = advisorRepo.GetAdvisorStudents(teacherId);
            dataGridLopCoVan.ItemsSource = list;
        }

        private void XemLichCoiThi_Click(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "LỊCH ĐI COI THI";
            ShowOnly(gridLichCoiThi);
            var repo = new ExamScheduleRepository();
            var data = repo.GetExamScheduleForTeacher(UserSession.FullName);

            dataGridLichCoiThi.ItemsSource = data;
        }

        private void Btn_saveGrades_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var repo = new GradeRepository();

                foreach (var row in dataGridDiem.Items)
                {
                    // BỎ QUA placeholder row
                    if (row == CollectionView.NewItemPlaceholder)
                        continue;
 
                    var item = row as GradeRepository.GradeRecord;
                    if (item == null)
                        continue;

                    repo.UpdateGrade(item);
                }

                MessageBox.Show("Lưu điểm thành công!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu điểm:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            // HIỆN FORM
            gridUpdateProfile.Visibility = Visibility.Visible;

            // ĐỔ SẴN DỮ LIỆU VÀO FORM
            txtEditFullName.Text = currentTeacher.FullName;
            cbEditGender.Text = currentTeacher.Gender;
            txtEditEmail.Text = currentTeacher.Email;
            txtEditAddress.Text = currentTeacher.Address;
            txtEditFaculty.Text = currentTeacher.Faculty;
        }

        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            var updated = new TeacherInfoModel
            {
                TeacherId = UserSession.UserId,
                FullName = txtEditFullName.Text.Trim(),
                Gender = cbEditGender.Text,
                Email = txtEditEmail.Text.Trim(),
                Address = txtEditAddress.Text.Trim(),
                Faculty = txtEditFaculty.Text.Trim()
            };

            var repo = new TeacherRepository();
            bool ok = repo.UpdateTeacher(updated);

            if (ok)
            {
                
                UserSession.FullName = updated.FullName;
                txtTenGV_Header.Text = $"Giảng viên: {UserSession.FullName}";
                txtTenGV_Left.Text = UserSession.FullName;
                LoadTeacherProfile();
                MessageBox.Show("Cập nhật thành công!");
                gridUpdateProfile.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Có lỗi khi cập nhật dữ liệu!");
            }
        }

        private void btnCancelProfile_Click(object sender, RoutedEventArgs e)
        {
            gridUpdateProfile.Visibility = Visibility.Collapsed;
        }

        private void TryShowGradeTable()
        {
            if (cbSemester.SelectedItem == null || cbClass.SelectedItem == null)
            {
                dataGridDiem.Visibility = Visibility.Collapsed;
                return;
            }

            string selectedClass = (cbClass.SelectedItem as CourseClassModel)?.CourseClassId;
            if (string.IsNullOrEmpty(selectedClass))
                return;

            var repo = new GradeRepository();
            var grades = repo.GetStudentGrades(selectedClass, UserSession.UserId);
            //dataGridDiem.AutoGenerateColumns = false;
            dataGridDiem.ItemsSource = grades;
            dataGridDiem.Visibility = Visibility.Visible;
        }

            
        private void cbSemester_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string semester = cbSemester.SelectedItem?.ToString();
            if (semester == null) return;

            var classes = courseRepo.GetClassesBySemester(UserSession.UserId, semester);

            cbClass.ItemsSource = classes;
            cbClass.DisplayMemberPath = "CourseClassId";

            TryShowGradeTable();
        }

        private void cbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedClass = (cbClass.SelectedItem as CourseClassModel)?.CourseClassId;
            if (selectedClass == null) return;

            var repo = new GradeRepository();
            var grades = repo.GetStudentGrades(selectedClass, UserSession.UserId);
            dataGridDiem.ItemsSource = grades;

            TryShowGradeTable();
        }


        private void btn_logOut_Click(object sender, RoutedEventArgs e)
        {
            UserSession.UserId = null;
            UserSession.FullName = null;

            Login lg = new Login();
            lg.Show();
            this.Close();
        }

        private void btn_noti_Click(object sender, RoutedEventArgs e)
        {
            txtTitle.Text = "";
            var data = notiService.LoadForTeacher();
            listNotification.ItemsSource = data;

            ShowOnly(gridNotification);
        }

        private void btn_changeAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Chọn ảnh đại diện";
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                string sourcePath = dlg.FileName;

                // Tạo thư mục Images/Avatars trong ứng dụng
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string avatarFolder = System.IO.Path.Combine(appFolder, "Images", "Avatars");

                if (!Directory.Exists(avatarFolder))
                    Directory.CreateDirectory(avatarFolder);

                // Đặt tên file theo teacher_id để không bị trùng
                string fileName = $"{UserSession.UserId}.png";
                string destinationPath = System.IO.Path.Combine(avatarFolder, fileName);

                // Copy file vào thư mục Images/Avatars (ghi đè nếu đã tồn tại)
                File.Copy(sourcePath, destinationPath, true);

                // Lưu vào DB chỉ tên file
                var repo = new TeacherRepository();
                repo.UpdateAvatar(UserSession.UserId, fileName);

                // Cập nhật lại UI
                LoadAvatar(destinationPath);

                // Lưu vào session
                UserSession.AvatarPath = destinationPath;

                MessageBox.Show("Cập nhật ảnh đại diện thành công!");
            }
        }

        private void LoadAvatar(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                Image img = new Image();
                img.Source = bitmap;
                img.Stretch = Stretch.Fill;

                imgAvatar.Child = img;
            }
            catch { }
        }

        private void GiaoBaiTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "GIAO BÀI TẬP";
            ShowOnly(gridGiaoBaiTap);

            // Load danh sách lớp mà GV đang dạy
            var repo = new CourseClassRepository();
            cbGiaoBT_Class.ItemsSource = repo.GetCourseClassesByTeacher(UserSession.UserId);
            cbGiaoBT_Class.DisplayMemberPath = "courseClass_id";

            var list = repo.GetCourseClassesByTeacher(UserSession.UserId);
            cbGiaoBT_Class.ItemsSource = list;
            cbGiaoBT_Class.DisplayMemberPath = "CourseClassId";
        }

        private void BtnGiaoBai_Click(object sender, RoutedEventArgs e)
        {
            if (cbGiaoBT_Class.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn lớp!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGiaoBT_Content.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung bài tập!");
                return;
            }

            if (dpGiaoBT_Start.SelectedDate == null || dpGiaoBT_End.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ ngày bắt đầu và kết thúc!");
                return;
            }

            var selectedClass = (CourseClassModel)cbGiaoBT_Class.SelectedItem;

            AssignmentModel am = new AssignmentModel
            {
                CourseClassId = selectedClass.CourseClassId,
                TeacherId = UserSession.UserId,
                Content = txtGiaoBT_Content.Text.Trim(),
                StartDate = dpGiaoBT_Start.SelectedDate.Value,
                EndDate = dpGiaoBT_End.SelectedDate.Value
            };

            var repo = new AssignmentRepository();
            repo.InsertAssignment(am);

            MessageBox.Show("Giao bài tập thành công!");

            txtGiaoBT_Content.Text = "";
            dpGiaoBT_Start.SelectedDate = null;
            dpGiaoBT_End.SelectedDate = null;
        }

        private void BtnGiaoBaiCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(gridNoiQuy);
        }

        private void OpenQuanLyAssignment()
        {
            ShowOnly(gridQuanLyAssignment);

            var repo = new AssignmentRepository();
            var list = repo.GetAssignments(UserSession.UserId);

            dataGridAssignment.ItemsSource = list;
        }

        private void QuanLyBaiTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           txtTitle.Text = "QUẢN LÝ BÀI TẬP";
            OpenQuanLyAssignment();
        }

        private void btnEditAssign_Click(object sender, RoutedEventArgs e)
        {
            dataGridAssignment.IsReadOnly = false;
            MessageBox.Show("Bạn có thể chỉnh sửa nội dung!", "Sửa");
        }

        private void btnDeleteAssign_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGridAssignment.SelectedItem as AssignmentModel;
            if (item == null)
            {
                MessageBox.Show("Hãy chọn bài tập cần xóa!");
                return;
            }

            var repo = new AssignmentRepository();
            repo.DeleteAssignment(item.Id);

            MessageBox.Show("Đã xóa!");

            OpenQuanLyAssignment();
        }

        private void btnSaveAssign_Click(object sender, RoutedEventArgs e)
        {
            var list = dataGridAssignment.ItemsSource as List<AssignmentModel>;
            var repo = new AssignmentRepository();

            foreach (var a in list)
            {
                repo.UpdateAssignment(a);
            }

            dataGridAssignment.IsReadOnly = true;

            MessageBox.Show("Đã lưu các thay đổi!");
        }

        private void cbAssignmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAssignmentList.SelectedValue == null)
                return;

            int assignmentId = (int)cbAssignmentList.SelectedValue;

            var repo = new StudentSubmitRepository();
            var list = repo.GetSubmissions(assignmentId);

            dataGridStudentSubmit.ItemsSource = list;
        }

        private void XemNopBai_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtTitle.Text = "XEM DANH SÁCH SINH VIÊN NỘP BÀI TẬP";
            ShowOnly(gridXemNopBai);

            var repo = new AssignmentRepository();
            var list = repo.GetAssignmentsByTeacher(UserSession.UserId);

            cbAssignmentList.ItemsSource = list;
        }

        private void LoadTeachingSchedule()
        {
            if (cbx_hockilichday.SelectedItem == null) return;
            if (cbx_namlichday.SelectedItem == null) return;

            string semester = cbx_hockilichday.SelectedItem?.ToString();
            string year = cbx_namlichday.SelectedItem?.ToString();


            var data = _courseRepo.GetTeachingSchedule(UserSession.UserId, semester, year);

            dataGridLichDay.ItemsSource = data;
            dataGridLichDay.Visibility = Visibility.Visible;
        }

        private void LoadClassList()
        {
            if (cbx_hockilopday.SelectedItem == null) return;
            if (cbx_namlopday.SelectedItem == null) return;

            string semester = cbx_hockilopday.SelectedItem.ToString();
            string year = cbx_namlopday.SelectedItem.ToString();

            var data = _courseRepo.GetTeachingSchedule(UserSession.UserId, semester, year);

            dataGridLopDay.ItemsSource = data;
            dataGridLopDay.Visibility = Visibility.Visible;
        }



        private void cbx_dslopday_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadClassList();
        }


        private void cbx_namlichday_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LoadTeachingSchedule();
        }

        private void cbx_hockilichday_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LoadTeachingSchedule();
        }

        private void cbx_hockilopday_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LoadClassList();
        }

        private void cbx_namlopday_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             LoadClassList();
        }

        //private void btn_timlichday_Click(object sender, RoutedEventArgs e)
        //{
        //    if (cbx_hockilichday.SelectedItem == null || cbx_namlichday.SelectedItem == null)
        //    {
        //        MessageBox.Show("Vui lòng chọn đủ học kỳ và năm.");
        //        return;
        //    }

        //    string semester = cbx_hockilichday.SelectedItem.ToString();
        //    string year = cbx_namlichday.SelectedItem.ToString();

        //    var repo = new TeachingScheduleRepository();
        //    var data = repo.GetTeachingSchedule(UserSession.UserId, semester, year);

        //    dataGridLichDay.ItemsSource = data;
        //    dataGridLichDay.Visibility = Visibility.Visible;
        //}

        private void btn_timlopday_Click(object sender, RoutedEventArgs e)
        {
            if (cbx_hockilopday.SelectedItem == null || cbx_namlopday.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đủ học kỳ và năm.");
                return;
            }

            string semester = cbx_hockilopday.SelectedItem.ToString();
            string year = cbx_namlopday.SelectedItem.ToString();

            var repo = new TeachingScheduleRepository();
            var data = repo.GetTeachingSchedule(UserSession.UserId, semester, year);

            dataGridLopDay.ItemsSource = data;
            dataGridLopDay.Visibility = Visibility.Visible;
        }

        private void btn_timlichday_Click(object sender, RoutedEventArgs e)
        {
            if (cbx_hockilichday.SelectedItem == null || cbx_namlichday.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đủ học kỳ và năm.");
                return;
            }

            string semester = cbx_hockilichday.SelectedItem.ToString();
            string year = cbx_namlichday.SelectedItem.ToString();

            var repo = new TeachingScheduleRepository();
            var data = repo.GetTeachingSchedule(UserSession.UserId, semester, year);

            dataGridLichDay.ItemsSource = data;
            dataGridLichDay.Visibility = Visibility.Visible;
        }

        private void btn_trangchu_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly(gridNoiQuy);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
