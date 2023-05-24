namespace Testing_System.Data.Entity
{

    public enum Difficulty
    {
        Beginner,
        Easy,
        Medium,
        Hard,
        Advanced,
        Deep
    }

    public class Question
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }

        public String Description { get; set; } = null!;

        public Difficulty Difficulty { get; set; }

        public String? ImageURL { get; set; }
    }
}
