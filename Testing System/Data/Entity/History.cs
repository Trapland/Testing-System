namespace Testing_System.Data.Entity
{
    public class History
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid AnswerId { get; set; }

        public Guid StudentId { get; set; }

        public Boolean Marked { get; set; }
    }
}
