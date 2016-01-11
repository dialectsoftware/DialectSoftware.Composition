using System;
using System.Text;
using System.Linq;
using System.Reflection;
using DialectSoftware.Composition;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace Conjure
{
    
    class Program
    {
       
        static void Main(string[] args)
        {
            IDo x = new B();
          
            #region TRAP
            x = Hoodoo.Conjure<IDo>(x)
                .Trap(typeof(IDo).Method(m => m.Name == "DoSomething"), (i, m, e) =>
                {
                    Console.WriteLine("*****ERROR********");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("******************\r\n");
                })
                .Cast();
            try
            {
               Console.WriteLine(x.DoSomething("echo"));
            }
            catch
            { 
            
            }
            #endregion

            #region INTERJECT
            Console.WriteLine("*****INTERJECT********");
            x = Hoodoo.Conjure<IDo>(x)
                .Interject(typeof(IDo).Method(m => m.Name == "DoSomething"), new Func<String, String>(s => { return s + " Look ma no error! "; }))
                .Cast();
            Console.WriteLine(x.DoSomething("echo"));
            Console.WriteLine("************************\r\n");
            #endregion

            #region OBSERVE
            x = Hoodoo.Conjure<IDo>(x)
                .Observe(typeof(IDo).Method(m => m.Name == "DoSomething"),
                    (i, m, o) =>
                    {
                        Console.WriteLine("*****OBSERVE PRE EXECUTE********");
                        Console.WriteLine("start " + m.ToString() + " on " + i.GetType().Name);
                        Console.WriteLine("*********************************\r\n");
                    },
                    (i, m, o) =>
                    {
                        Console.WriteLine("*****OBSERVE POST EXECUTE********");
                        if(o==null)
                            Console.WriteLine("end " + m.ToString() + " on " + i.GetType().Name  + " == null");
                        else
                            Console.WriteLine("end " + m.ToString() + " on " + i.GetType().Name + " == " + o.ToString());
                        Console.WriteLine("**********************************\r\n");
                        
                    })
               .Cast();
            try
            {
                Console.WriteLine(x.DoSomething("echo"));
            }
            catch
            {

            }
            #endregion

            IDo y = new A();

            #region PREEMPT
            Console.WriteLine("DoSomething says " + y.DoSomething("Y was set"));
            Console.WriteLine("y.Test == \"" + y.Test + "\"\r\n");

            y = Hoodoo.Conjure<IDo>(y)
                .Trap(typeof(IDo).Method(m => m.Name == "get_Test"), (i, m, e) =>
                {
                    Console.WriteLine(e.ToString());
                })
                .Observe(typeof(IDo)
                .Method(p => p.Name == "get_Test"),
                (i, m, o) =>
                {
                    Console.WriteLine("*****OBSERVE PRE EXECUTE********");
                    Console.WriteLine("start " + m.ToString() + " " + i.GetType().Name);
                    Console.WriteLine("*********************************\r\n");
                },
                (i, m, o) =>
                {
                    Console.WriteLine("*****OBSERVE POST EXECUTE********");
                    if (o == null)
                        Console.WriteLine("end " + m.ToString() + " on " + i.GetType().Name + "== null");
                    else
                        Console.WriteLine("end " + m.ToString() + " on " + i.GetType().Name + " == \"" + o.ToString() + "\"");
                    Console.WriteLine("**********************************\r\n");
                })
                .Preempt(typeof(IDo).Method(m => m.Name == "DoSomething"), 
                    new Func<String, String>(s => 
                        {
                            Console.WriteLine("*****PREEMPTED********");
                            s += " *BUT NOTE*: get_Test() property remains unchanged \r\n";
                            Console.WriteLine("**********************\r\n");
                            return s;
                        }))
                .Cast();

            Console.WriteLine("DoSomething says " + y.DoSomething("Y was updated"));
            Console.WriteLine("y.Test == \"" + y.Test  + "\"\r\n");
            
            #endregion

            #region PREDICATE
            y = Hoodoo.Conjure<IDo>(y).Predicate(typeof(IDo).Method(m => m.Name == "DoSomething"), 
                (i,m,o)=>
                {
                    Console.WriteLine("*****PREDICATE******\r\n");
                    return o.Length != 1 || o[0] == null || o[0].ToString().Length < 4;
                },
            new Func<String, String>(s=>{

                throw new Exception("String must be 4 or more characters long");
            
            })).Cast();

            try
            {
                Console.WriteLine("DoSomething says " + y.DoSomething("This is > 4 characters carry on\r\n"));
                Console.WriteLine("DoSomething says " + y.DoSomething("< 4"));
            }
            catch(Exception e)
            { 
                Console.WriteLine("ERROR: " + e.Message);
            }
            #endregion

            Console.ReadLine();
        }
    }


   

}
