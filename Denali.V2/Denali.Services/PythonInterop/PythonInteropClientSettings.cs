using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.PythonInterop
{
    public class PythonInteropClientSettings
    {
        public static string Settings = "PythonInteropClientSettings";

        public string BaseAddress { get; set; }
        public string OLSPath { get; set; }
    }
}
