using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class Pupil
    {
        public Pupil(int id, string pupilLName, string pupilFName, string pupilSurname, string className)
        {
            Id = id;
            PupilLName = pupilLName;
            PupilFName = pupilFName;
            PupilSurname = pupilSurname;
            ClassName = className;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName("Фамилия")]
        public string PupilLName { get; set; }
        [DisplayName("Имя")]
        public string PupilFName { get; set; }
        [DisplayName("Отчество")]
        public string PupilSurname { get; set; }
        [DisplayName("Класс")]
        public string ClassName { get; set; }
    }
}
