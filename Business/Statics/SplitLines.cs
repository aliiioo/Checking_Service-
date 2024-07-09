namespace Business.Statics
{
    public static class SplitLines
    {
        public static string[] SplitLine(string line)
        {
            return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
