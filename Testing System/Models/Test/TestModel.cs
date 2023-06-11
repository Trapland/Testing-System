namespace Testing_System.Models.Test
{
    public class TestModel
    {

        public String Id { get; set; } = null!;

        public String? Name { get; set; }

        public String? Description { get; set; }

        public String TeacherId { get; set; } = null!;

        public String StudentId { get; set; } = null!;

        public int StartCount { get; set; }

        public int Count { get; set; }

        public TimeSpan Time { get; set; }

        public List<QuestionModel> Questions { get; set; } = null!;

        public List<AnswerViewModel> Answers { get; set; }
    }
}
