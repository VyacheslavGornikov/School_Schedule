using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class Teacher
    {
        public Teacher(int id, string teacherLName, string teacherFName, string teacherSurname)
        {
            Id = id;
            TeacherLName = teacherLName;
            TeacherFName = teacherFName;
            TeacherSurname = teacherSurname;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName("Фамилия")]
        public string TeacherLName { get; set; }
        [DisplayName("Имя")]
        public string TeacherFName { get; set; }
        [DisplayName("Отчество")]
        public string TeacherSurname { get; set; }
    }
}
