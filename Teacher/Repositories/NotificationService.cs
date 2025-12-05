using System.Collections.Generic;
using QuanLySinhVien.Models;
using QuanLySinhVien.Repositories;

public class NotificationService
{
    NotificationRepository repo = new NotificationRepository();

    public List<NotificationModel> LoadForTeacher()
    {
        return repo.GetNotifications("Giảng viên");
    }
}
