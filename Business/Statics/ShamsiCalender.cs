namespace Business.Statics
{
    public static class ShamsiCalender
    {
        public static string ToShamsi(DateTime dateTime)

        {
            System.Globalization.PersianCalendar persianCalandar =
                                    new System.Globalization.PersianCalendar();
            int year = persianCalandar.GetYear(dateTime);
            int month = persianCalandar.GetMonth(dateTime);
            int day = persianCalandar.GetDayOfMonth(dateTime);
            string shamsi = $"{year}/{month}/{day}";

            return shamsi;

        }
    }
}
