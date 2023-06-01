using Testing_System.Data.Entity;

namespace Testing_System.Models.Test
{
    public class CreateQuestionModel
    {
        public String TestId { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String? ImageURL { get; set; }

        public String Difficulty { get; set; } = null!;

        public int counter { get; set; } = 1;

        public String Answer1 { get; set; } = null!;

        public int ValueAnswer1 { get; set; }

        public String Answer2 { get; set; } = null!;

        public int ValueAnswer2 { get; set; }

        public String Answer3 { get; set; } = null!;

        public int ValueAnswer3 { get; set; }

        public String Answer4 { get; set; } = null!;

        public int ValueAnswer4 { get; set; }

    }
}
