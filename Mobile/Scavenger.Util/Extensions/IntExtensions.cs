using System;

namespace Scavenger.Util.Extensions
{
	public static class IntExtensions
	{
		public static int RoundToLowestHundreds (this int i)
		{
			return (int)Math.Floor(i / 100.0) * 100;
		}
	}
}

