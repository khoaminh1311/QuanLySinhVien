using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySinhVien.Utils
{
    public static class UserSession
    {
        public static string UserId { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }
        public static string AvatarPath { get; set; }
    }
}
