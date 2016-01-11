using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace Conjure
{
    public class B : A
    {
        public override string DoSomething(string m)
        {
            //*****WHEN YOU ENCOUNTER THIS ERRROR IN DEBUG MODE JUST HIT F5 TO CONTINUE****
            throw new NotImplementedException();
        }

    }
}
