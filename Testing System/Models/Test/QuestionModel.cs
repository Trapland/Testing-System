namespace Testing_System.Models.Test
{
    public class QuestionModel
    {
        public String Id { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String? ImageURL { get; set; }

        public List<AnswerModel> Answers { get; set; } = null!;
    }
}
