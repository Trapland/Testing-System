namespace Testing_System.Data.Entity
{
    public class Answer
    {
        public Guid Id { get; set; }

        public Guid QusetionId { get; set; }

        public String Description { get; set; } = null!;

        public int Value { get; set; }
    }
}
