﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class MyException : ApplicationException
    {
        public MyException(string message) : base(message) { }
    }
}
