namespace Testing_System.Models.User
{
    public class StudentProfileModel : ProfileModel
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public String Login { get; set; }

        public String Email { get; set; }

        public String Avatar { get; set; }


        public StudentProfileModel(Data.Entity.Student student)
        {
            var thisProps = this.GetType().GetProperties();
            foreach (var prop in student.GetType().GetProperties())
            {
                var thisProp = thisProps.FirstOrDefault(p =>
                p.Name == prop.Name && p.PropertyType.IsAssignableFrom(prop.PropertyType));
                if (thisProp is not null)
                {
                    thisProp?.SetValue(this, prop.GetValue(student));
                }
            }
        }
    }
}
