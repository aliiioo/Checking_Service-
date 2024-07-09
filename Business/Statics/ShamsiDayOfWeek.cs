namespace Business.Statics
{
    public static class ShamsiDayOfWeek
    {
        public static string GetDayShamsi(string days)
        {
            switch (days)
            {
                case "Monday":
                    return "دوشنبه";
                case "Tuesday":
                    return "سه شنبه ";
                case "Wednesday":
                    return "چهار شنبه";
                case "Thursday":
                    return "پنجشنبه";
                case "Friday":
                    return "جمعه";
                case "Saturday":
                    return "شنبه";
                case "Sunday":
                    return "یک شنبه";
                default:
                    return "خطای روز";
            }
        }






    }
}
