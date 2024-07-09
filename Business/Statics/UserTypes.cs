using Infrastructure.Models;

namespace business.Statics
{
    public static class TypeOfKind
    {
        public static string GetTypeName(type type)
        {
            switch (type)
            {
                case type.Normal:
                    return "عادی";
                case type.hourly_leave:
                    return "مرخصی ساعتی";
                case type.delay:
                    return "تاخیر";
                case type.daily_leave:
                    return "مرخصی روزانه";
                case type.error:
                    return "خطا";
                default:
                    return "غیرقابل پیش بینی";
            }
        }

    }
}
