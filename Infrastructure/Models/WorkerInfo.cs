using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public int row { get; set; }
        public DateTime DateTime { get; set; }
        public string Day { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<type>? Status { get; set; } = new List<type>(); 
        public TimeSpan WorkTime { get; set; }
        public TimeSpan FirstArrive { get; set; }
        public TimeSpan StartTimer { get; set; } = new TimeSpan(0);
        public TimeSpan LastCheckout { get; set; }
        public List<string> Record { get; set; } = new List<string>();
        public HashSet<string> RealRecord { get; set; } = new HashSet<string>();
    }


    public enum type
    {
        Normal,
        hourly_leave,
        delay,
        daily_leave,
        error

    }
}
