namespace Testing_System.Models.Test
{
    public class CreateTestModel
    {
        public String Id { get; set; } = null!;

        public String Name { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String TeacherId { get; set; } = null!;

        public int Count { get; set; }

        public TimeSpan Time { get; set; }

        public int StartCount { get; set; }
    }
}
