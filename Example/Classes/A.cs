using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conjure
{
    public class A : IDo
    {
        public string Test
        {
            get;
            private set;
        }

        public virtual string DoSomething(string m)
        {
            Test = m;
            return m;
        }
    }
}
