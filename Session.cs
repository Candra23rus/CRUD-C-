using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiSekolah
{
    public static class Session
    {
        public static int UserID { get; set; } = 0; // Default 0
        public static string FullName { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;

        public static void Logout()
        {
            UserID = 0;
            FullName = string.Empty;
            Role = string.Empty;
        }
    }

}
