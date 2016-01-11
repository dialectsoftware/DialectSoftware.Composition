using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conjure
{
    public interface IDo
    {
        string Test { get; }
        string DoSomething(string m);
    }
}
