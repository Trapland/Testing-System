namespace Testing_System.Models.User
{
    public class Registration
    {
        public String Login { get; set; } = null!;

        public String Email { get; set; } = null!;

        public String Password { get; set; } = null!;

        public String RepeatPassword { get; set; } = null!;

        public String Name { get; set; } = null!;

        public String Surname { get; set; } = null!;

        public IFormFile Avatar { get; set; } = null!;

        public String Option { get; set; } = null!;

    }
}
