namespace Testing_System.Models.User
{
    public class TeacherProfileModel : ProfileModel
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public String Login { get; set; }

        public String Email { get; set; }

        public String Avatar { get; set; }


        public TeacherProfileModel(Data.Entity.Teacher teacher)
        {
            var thisProps = this.GetType().GetProperties();
            foreach (var prop in teacher.GetType().GetProperties())
            {
                var thisProp = thisProps.FirstOrDefault(p =>
                p.Name == prop.Name && p.PropertyType.IsAssignableFrom(prop.PropertyType));
                if (thisProp is not null)
                {
                    thisProp?.SetValue(this, prop.GetValue(teacher));
                }
            }
        }
    }
}
