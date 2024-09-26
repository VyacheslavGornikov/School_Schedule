using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class Class
    {
        public Class(int id, string className)
        {
            Id = id;
            ClassName = className;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName("Класс")]
        public string ClassName { get; set; }
    }
}
