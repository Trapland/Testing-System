namespace Testing_System.Services.Random
{
    public class RandomService : IRandomService
    {
        private readonly String _safeChars = new String(Enumerable.Range(20, 107).Select(x => (char)x).ToArray());
        private readonly System.Random _random = new();

        public string RandomString(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _safeChars[_random.Next(_safeChars.Length)];
            }
            return new String(chars);
        }
    }
}
