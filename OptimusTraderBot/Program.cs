using System;
using Microsoft.Extensions.Configuration;
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

			Console.WriteLine($"{apiSettings.PrivateApiKey}");
			Console.Read();
		}

	}
}
