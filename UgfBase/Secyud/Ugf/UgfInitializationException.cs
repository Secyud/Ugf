#region

using System;
using System.Runtime.Serialization;

#endregion

namespace Secyud.Ugf
{
	public class UgfInitializationException : UgfException
	{
		public UgfInitializationException()
		{
		}

		public UgfInitializationException(string message)
			: base(message)
		{
		}

		public UgfInitializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public UgfInitializationException(SerializationInfo serializationInfo, StreamingContext context)
			: base(serializationInfo, context)
		{
		}
	}
}