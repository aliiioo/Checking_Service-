namespace Business.Statics
{
    public static class LawOfTime
    {
        public static readonly TimeSpan MinOfArrive = new(8, 30, 0);    
        public static readonly TimeSpan MinOfCheckout = new(17, 0, 0);    
        public static readonly TimeSpan MaxOfArrive = new(8, 45, 0);    
        public static readonly TimeSpan MaxOfChckout = new(17, 15, 0);
        public static readonly TimeSpan ZeroTime = new(0, 0, 0);
        public static readonly TimeSpan StandarTime = new(8, 30, 0);

    }
}
