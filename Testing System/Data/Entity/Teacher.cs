namespace Testing_System.Data.Entity
{
    public class Teacher
    {
        public Guid Id { get; set; }

        public String Name { get; set; } = null!;

        public String Surname { get; set; } = null!;

        public String Email { get; set; } = null!;

        public String Login { get; set; } = null!;

        public String PasswordHash { get; set; } = null!;

        public String PasswordSalt { get; set; } = null!;

        public String? Avatar { get; set; }
    }
}
