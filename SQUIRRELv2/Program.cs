using System;

namespace SQUIRRELv2
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
		}
		public static string clearformatting (string input)
		{
			// adds 0-width characters to the beginning of MD characters - ending is a mess because RTL char
			var output = input.Replace("`", "​`").Replace("*", "​*").Replace("_", "​_").Replace("‮", "");
			return output;
		}
	}
}
