using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class Grade
    {
        public Grade(int id, string pupilLName, string pupilFName, string pupilSurname, int quarter, string lesson_grade, int homework_grade)
        {
            Id = id;
            PupilLName = pupilLName;
            PupilFName = pupilFName;
            PupilSurname = pupilSurname;
            Quarter = quarter;
            Lesson_grade = lesson_grade;
            Homework_grade = homework_grade;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName("Фамилия")]
        public string PupilLName { get; set; }
        [DisplayName("Имя")]
        public string PupilFName { get; set; }
        [DisplayName("Отчество")]
        public string PupilSurname { get; set; }
        [DisplayName("Четверть")]
        public int Quarter { get; set; }
        [DisplayName("Оценка за урок/Посещаемость")]
        public string Lesson_grade { get; set; }
        [DisplayName("Оценка за домашнее задание")]
        public int Homework_grade { get; set; }
        
    }
}
