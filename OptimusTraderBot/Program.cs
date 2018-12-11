using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OptimusTraderBot.Connector;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var apiSettings = new ApiSettings();
			config.GetSection("ApiSettings").Bind(apiSettings);

			var apiConnector = new ApiConnector(apiSettings);

			string result =apiConnector.CallApiOperation(ApiMethod.Info, new Dictionary<string, string>()).Result;
			string json = JToken.Parse(result).ToString(Formatting.Indented);

			Console.WriteLine(json);
			Console.Read();
		}

	}
}
