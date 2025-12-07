using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management_system
{
    class Admin
    {
        private static string adminId;
        private static string adminName;
        private static string gender;
        private static string email;
        private static string phone;
        private static string address;

        public static string AdminId { get => adminId; set => adminId = value; }
        public static string AdminName { get => adminName; set => adminName = value; }
        public static string Gender { get => gender; set => gender = value; }
        public static string Email { get => email; set => email = value; }
        public static string Phone { get => phone; set => phone = value; }
        public static string Address { get => address; set => address = value; }
    }
}
