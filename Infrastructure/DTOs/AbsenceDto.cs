using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class AbsenceDto
    {
        public DateTime CurrentDate { get; set; }
        public List<UserInfo> ListUserInfo { get; set; } = new List<UserInfo>();
        public Dictionary<int, string> Absence { get; set; } = new Dictionary<int, string>();
        public int Row { get; set; }
    }

}
