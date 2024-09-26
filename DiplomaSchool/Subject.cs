using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class Subject
    {
        public Subject(int id, string subjectName)
        {
            Id = id;
            SubjectName = subjectName;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName("Предмет")]
        public string SubjectName { get; set; }
    }
}
