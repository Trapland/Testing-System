namespace Testing_System.Models.Test
{
    public class TestHistoryViewModel
    {
        public String Id { get; set; } = null!;

        public String? Name { get; set; }

        public String? Description { get; set; }

        public int Count { get; set; }

        public StudentViewModel student { get; set; } = null!;

        public String SessionId { get; set; } = null!;
    }
}
