using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Client.IO
{
    interface IReader
    {
        byte[] ReadAllBytes(string path);
        string ReadAllText(string path);
        string[] ReadAllLines(string path);
    }
}
