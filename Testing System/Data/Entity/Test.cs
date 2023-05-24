namespace Testing_System.Data.Entity
{
    public class Test
    {
        public Guid Id { get; set; }

        public String Name { get; set; } = null!;

        public String Description { get; set; } = null!;

        public Guid TeacherId { get; set; }

        public int Count { get; set; }

        public TimeSpan Time { get; set; }

        public int StartCount { get; set; } 
    }
}
