using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace $safeprojectname$.Infrastructure
{
    public class SerilogExcludeRule
    {
        public Regex Regex { get; set; }
    }
}
