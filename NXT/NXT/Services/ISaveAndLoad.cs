using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXT.Services
{
    public interface ISaveAndLoad
    {
        Task WriteAllTextAsync(string filename, string data);
        Task<string> ReadAllTextAsync(string fileName);
    }
}
