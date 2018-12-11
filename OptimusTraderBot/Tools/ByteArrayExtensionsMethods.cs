using System;

namespace OptimusTraderBot.Tools
{
		public static class ByteArrayExtensionMethods
		{
			public static string ToHexString(this byte[] ba) => BitConverter.ToString(ba).Replace("-", "").ToLower();
		}
}
