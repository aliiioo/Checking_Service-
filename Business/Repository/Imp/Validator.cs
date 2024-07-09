using business.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business.Repository.Imp
{
    public class Validator : IValidator
    {
        public DateTime? ValidDate(string dateString)
        {
            string Format = "yyyy/MM/dd";
            CultureInfo Provider = CultureInfo.InvariantCulture;
            var isValid = DateTime.TryParseExact(dateString, Format, Provider, DateTimeStyles.None, out DateTime Date);

            if (isValid)
            {
                if (Date <= DateTime.UtcNow)
                {
                    return Date;
                }
            }
            return null;
        }

        public int? ValidId(string idString)
        {
            var IsVAlid = int.TryParse(idString, out int Id);
            if (IsVAlid)
            {
                if (Id > 0)
                {
                    return Id;
                }
            }
            return null;
        }

        public bool ValidNname(string Name)
        {
            string pattern = @"^[a-zA-Z]+$";
            bool IsValid = Regex.IsMatch(Name, pattern);
            if (IsValid)
            {
                return true;
            }
            return false;
        }

        public TimeSpan? ValidTime(string timeString)
        {
            //// Time Like 018:00:00 Or 00:033:00
            var Timer = timeString.Split(':');
            if (Timer.Length != 3)
            {
                return null;
            }
            var Hour = (int.Parse(Timer[0]));
            var Min = (int.Parse(Timer[1]));
            var Sec = (int.Parse(Timer[2]));
            timeString = new TimeSpan(Hour, Min, Sec).ToString();
            string Format = @"hh\:mm\:ss";
            CultureInfo Provider = CultureInfo.InvariantCulture;


            bool IsValid = TimeSpan.TryParseExact(timeString, Format, Provider, out TimeSpan Time);

            if (IsValid)
            {
                return Time;
            }
            else
            {
                return null;
            }
        }
    }
}
