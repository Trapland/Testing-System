﻿using Testing_System.Models.Test;

namespace Testing_System.Models.User
{
    public interface ProfileModel
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public String Login { get; set; }

        public String Email { get; set; }

        public String Avatar { get; set; }

        public String UserStatus { get; set; }

        public List<TestViewModel> Tests { get; set; }

    }
}
