using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    internal class ConnectionParameters
    {
        public static string ConnectionString()
        {
            return @"Data Source=..\SchoolDB.db;Version=3;";
        }        
    }
}
