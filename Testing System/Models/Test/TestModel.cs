namespace Testing_System.Models.Test
{
    public class TestModel
    {

        public String Id { get; set; } = null!;

        public String TeacherId { get; set; } = null!;

        public List<QuestionModel> Questions { get; set; } = null!;
    }
}
