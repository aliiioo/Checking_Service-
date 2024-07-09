using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.Repository.Interface
{
    public interface IValidator
    {
        public TimeSpan? ValidTime(string timeString);
        public int? ValidId(string idString);
        public DateTime? ValidDate(string dateString);
        public bool ValidNname(string Name);

    }
}
