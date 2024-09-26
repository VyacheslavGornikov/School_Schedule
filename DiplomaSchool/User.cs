using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class User
    {
        public User()
        {
        }

        public User(int id, string login, string password, string userType, string userLName, string userFName, string userSurname)
        {
            Id = id;
            Login = login;
            Password = password;
            UserType = userType;
            UserLName = userLName;
            UserFName = userFName;
            UserSurname = userSurname;
        }

        public int Id { get; set; }
        [DisplayName("Логин")]
        public string Login { get; set; }
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [DisplayName("Тип пользователя")]
        public string UserType { get; set; }
        [DisplayName("Фамилия")]
        public string UserLName { get; set; }
        [DisplayName("Имя")]
        public string UserFName { get; set; }
        [DisplayName("Отчество")]
        public string UserSurname { get; set; }

    }
}
