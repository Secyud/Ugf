using System;

namespace Secyud.Ugf
{
	public static class UgfUnityCodes
	{
		public static string TypeMismatch(Type target, Type need)
		{
			return $"{target} is not a valid {need}!";
		}
	}
}