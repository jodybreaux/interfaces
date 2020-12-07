using System;
using System.Collections.Generic;
using System.Text;

namespace CreateSampleInputFile
{
    public class AppConfiguration
    {
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Connections
        {
            public string SqlConn { get; set; }
        }

        public class TargetFile
        {
            public string path { get; set; }
            public string filename { get; set; }
        }
    }
}
