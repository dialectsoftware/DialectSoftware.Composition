using System;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting;
using System.EnterpriseServices;
using System.Security.Permissions;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging; 


namespace DialectSoftware.Composition
{
	/// <summary>
	///	All exception generated in the runtime are converted to FoundationException.
	///	Aa result System.Exception being thrown by the Runtime indicates a fundamental 
	///	flaw in the Foundation.
	/// </summary>
	public class CompositionException:System.Exception
	{
		private string _message = String.Empty;

		public CompositionException():base()
		{
			_message = base.Message;
		}

		public CompositionException(string message):base(message)
		{
			_message = base.Message;
		}

		public CompositionException(string message, System.Exception innerException):base(message,innerException)
		{
			_message = base.Message;
		}

		public CompositionException(System.Type Type,string message, System.Exception innerException):this(String.Format("Exception:{0}\r\nAssembly:{1}\r\n{2}:{3}\r\n",message,Type.AssemblyQualifiedName,Type.GetConstructor(new System.Type[0]{}).MemberType.ToString(),Type.GetConstructor(new System.Type[0]{}).Name),innerException)
		{
			
		}

		public CompositionException(System.Reflection.MethodBase Method,string message, System.Exception innerException):this(String.Format("{0}\r\nAssembly:{1}\r\n{2}:{3}\r\nArgs:\r\n",message,Method.ReflectedType.AssemblyQualifiedName,Method.MemberType.ToString(),Method.Name),innerException)
		{
			System.Reflection.ParameterInfo[] parameters = Method.GetParameters();
			for(int i = 0; i<parameters.Length; i++)
			{
				_message = String.Format("{0}\t{1}({2})\r\n",_message,parameters[i].Name,parameters[i].ParameterType.ToString());
			}
		}

		public CompositionException(IMethodCallMessage Message,string message, System.Exception innerException):this(String.Format("Exception:{0}\r\nAssembly:{1}\r\n{2}:{3}\r\nArgs:\r\n",message,Message.TypeName,Message.MethodBase.MemberType,Message.MethodName),innerException)
		{
			for(int i=0; i<Message.ArgCount; i+=2)
			{
				_message = String.Format("{0}\t{1}={2}\r\n",_message,Message.GetArgName(i),Message.GetArg(i).ToString());
			}
			
		}
		
		public override string Message
		{
			get{return _message;}
		
		}
	
	}

}
