using System;
using Microsoft.Extensions.Configuration;
using OptimusTraderBot.Models;
using OptimusTraderBot.Settings;

namespace OptimusTraderBot
{
	internal partial class Program
	{
		private static void Main(string[] args)
		{
			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var apiSettings = new ApiSettings();
			config.GetSection("ApiSettings").Bind(apiSettings);

			var apiManager = new ApiManager(apiSettings);

			InfoResult info = apiManager.GetInfo();
			if(info.Success)
			{
				if(info.Balances.TryGetValue("LSK", out Money lsk))
				{
					Console.WriteLine($"LSK: {lsk}");
				}
				if(info.Addresses.TryGetValue("BTC", out string btcAddress))
				{
					Console.WriteLine(btcAddress);
				}
			}

			Console.Read();
		}
	}
}
