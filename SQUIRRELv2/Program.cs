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
			var output = "[empty string]";
			if(!string.IsNullOrWhiteSpace(input))
				output = input.Replace("`", "​`").Replace("*", "​*").Replace("_", "​_").Replace("‮", " ");
			return output;
		}
	}
}
