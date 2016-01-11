using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public interface IPreemptive<T>
{
    T Preempt(MethodInfo m, Delegate U);
    T Preempt(PropertyInfo p, Delegate U);
}