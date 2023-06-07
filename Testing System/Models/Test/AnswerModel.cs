namespace Testing_System.Models.Test
{
    public class AnswerModel
    {
        public String Id { get; set; } = null!;

        public int Value { get; set; }

        public String Description { get; set; } = null!;

        public bool isMax { get; set; } = false;

        public bool isMarked { get; set; } = false;



    }
}
