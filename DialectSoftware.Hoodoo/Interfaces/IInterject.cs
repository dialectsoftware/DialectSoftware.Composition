using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public interface IInterject<T>
{
     T Interject(MethodInfo m, Delegate U);
     T Interject(PropertyInfo p, Delegate U);
}
