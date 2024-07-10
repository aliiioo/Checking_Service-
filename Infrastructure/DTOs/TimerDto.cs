using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class TimerDto
    {
        public TimeSpan FirstIn { get; set; }
        public TimeSpan LastOut { get; set; }
        public TimeSpan WorkTime { get; set; }

    }
}
